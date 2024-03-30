// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CppAst;
using CppAst.CodeGen.Common;
using CppAst.CodeGen.CSharp;
using Microsoft.VisualBasic.FileIO;
using Zio.FileSystems;

namespace NPlug.CodeGen;

/// <summary>
/// Code generator from the VST C++ Sdk files to NPlug. This class mainly handle:
/// - Convert COM interfaces methods with proper proxy trampoline
/// - Convert structs that are used by these interfaces
/// - Collect COM IID for all interfaces
/// - Collect various enum definitions
/// </summary>
public class CodeGenerator
{
    private readonly string _sdkFolder;
    private readonly string _pluginInterfacesFolder;
    private string _destinationFolder;
    private readonly Dictionary<string, CSharpElement> _generatedCSharpElements;
    private readonly Dictionary<string, Uuid> _nameToIID;
    private readonly Dictionary<string, string[]> _fileToContent;
    private readonly Dictionary<string, CSharpType> _cppTypeToCSharpType;
    private readonly HashSet<string> _hostOnly;
    private readonly HashSet<string> _pluginOnly;
    private readonly HashSet<string> _typesToExclude;
    private readonly List<CSharpGeneratedFile> _toManagedFiles;
    private readonly List<CSharpStruct> _allGeneratedVtbls;
    private CSharpClass? _container;


    public CodeGenerator(string sdkFolder)
    {
        // Force these components to be host only
        _hostOnly = new HashSet<string>()
        {
            "IBStream",
            "IComponentHandler3",
            "IStringResult",
        };
        // Force these components to be plugin only
        _pluginOnly = new HashSet<string>()
        {
            "IPluginFactory2",
            "IPluginFactory3"
        };
        // The following types don't seem to be used in the VST3 SDK itself
        _typesToExclude = new HashSet<string>()
        {
            "ICloneable",
            "IDependent",
            "IErrorContext",
            "IInterAppAudioConnectionNotification",
            "IPersistent",
            "IPluginCompatibility",
            "ISizeableStream",
            "IString",
            "IUpdateHandler",
            "funknown"
        };
        _nameToIID = new Dictionary<string, Uuid>();
        _generatedCSharpElements = new Dictionary<string, CSharpElement>();
        _fileToContent = new Dictionary<string, string[]>();
        _cppTypeToCSharpType = new Dictionary<string, CSharpType>();
        _toManagedFiles = new List<CSharpGeneratedFile>();
        _allGeneratedVtbls = new List<CSharpStruct>();
        _sdkFolder = sdkFolder;
        _pluginInterfacesFolder = Path.Combine(_sdkFolder, "pluginterfaces");
        _destinationFolder = string.Empty;
        if (!Directory.Exists(_pluginInterfacesFolder))
        {
            throw new InvalidOperationException($"Directory {_pluginInterfacesFolder} not found. This doesn't look like a valid VST3 SDK folder.");
        }
    }

    public void Generate(string destinationFolder)
    {
        _destinationFolder = destinationFolder;
        // Setup input C++ header files and folders
        var baseFolder = Path.Combine(_pluginInterfacesFolder, "base");
        var guiFolder = Path.Combine(_pluginInterfacesFolder, "gui");
        var vstFolder = Path.Combine(_pluginInterfacesFolder, "vst");

        var headerFiles =
            Directory.EnumerateFiles(baseFolder, "*.h").Concat(Directory.EnumerateFiles(guiFolder, "*.h")).Concat(Directory.EnumerateFiles(vstFolder, "*.h")).ToList();
        // Remove the following files that are only meant to be used within the other files.
        headerFiles.RemoveAll(match => match.Contains("falignpop.h") || match.Contains("falignpush.h") || match.Contains("vstpshpack4.h"));

        // Parse the VST SDK C++ files
        var options = new CppParserOptions()
        {
            IncludeFolders = { _sdkFolder },
            ParseMacros = true,
        };
        options.ConfigureForWindowsMsvc(CppTargetCpu.X86_64);

        var cppAst = CppAst.CppParser.ParseFiles(headerFiles, options);
        if (cppAst.HasErrors)
        {
            foreach (var cppDiagnosticMessage in cppAst.Diagnostics.Messages)
            {
                Console.WriteLine(cppDiagnosticMessage);
            }
            return;
        }

        // Prepare for the C# generated file
        var csFile = new CSharpGeneratedFile("/LibVst.generated.cs");
        csFile.Members.Add(new CSharpFreeMember() { Text = @"#pragma warning disable CS0649
#pragma warning disable CS1658
#pragma warning disable CS1570
#pragma warning disable CS1573
#pragma warning disable CS1574
#pragma warning disable CS1584
" });
        csFile.Members.Add(new CSharpNamespace("NPlug.Interop") { IsFileScoped = true });
        csFile.Members.Add(new CSharpUsingDeclaration("System.Collections.Generic"));
        csFile.Members.Add(new CSharpUsingDeclaration("System.Diagnostics.CodeAnalysis"));
        csFile.Members.Add(new CSharpUsingDeclaration("System.Runtime.CompilerServices"));
        csFile.Members.Add(new CSharpUsingDeclaration("System.Runtime.InteropServices"));

        var csLibVst = new CSharpClass("LibVst")
        {
            Modifiers = CSharpModifiers.Static | CSharpModifiers.Partial,
            Visibility = CSharpVisibility.Internal
        };
        csFile.Members.Add(csLibVst);

        // Process the AST to generate the C# file
        _container = csLibVst;
        Process(cppAst);

        // Write the C# Files back to disk
        var fs = new PhysicalFileSystem();
        var subFolder = fs.ConvertPathFromInternal(destinationFolder);
        var subFs = new SubFileSystem(fs, subFolder);

        var codeWriter = new CodeWriter(new CodeWriterOptions(subFs));
        csFile.DumpTo(codeWriter);
        foreach (var ccwFile in _toManagedFiles)
        {
            ccwFile.DumpTo(codeWriter);
        }
    }

    private static bool IsUnknown(CppClass type)
    {
        if (type.Name == "FUnknown") return true;
        if (type.Functions.Any(x => (x.Flags & CppFunctionFlags.Pure) == 0))
        {
            return false;
        }

        foreach (var baseType in type.BaseTypes)
        {
            if (baseType.Type is CppClass cppClass)
            {
                if (IsUnknown(cppClass))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void Process(CppCompilation cppAst)
    {
        var macroSdkVersion = cppAst.Macros.FirstOrDefault(x => x.Name == "kVstVersionString");
        if (macroSdkVersion == null)
        {
            throw new InvalidOperationException("Unable to find sdk version");
        }
        Console.WriteLine($"Processing {macroSdkVersion.Value}");

        // Add SdkVersion field
        _container!.Members.Add(new CSharpField("SdkVersion") { FieldType = CSharpPrimitiveType.String(), Modifiers = CSharpModifiers.Const, Visibility = CSharpVisibility.Public , InitValue = macroSdkVersion.Value});

        // AudioEffectCategory
        var macro = cppAst.Macros.FirstOrDefault(x => x.Name == "kVstAudioEffectClass")!;
        _container!.Members.Add(new CSharpField("AudioEffectCategory") { FieldType = CSharpPrimitiveType.String(), Modifiers = CSharpModifiers.Const, Visibility = CSharpVisibility.Public, InitValue = macro.Value });
        macro = cppAst.Macros.FirstOrDefault(x => x.Name == "kVstComponentControllerClass")!;
        _container!.Members.Add(new CSharpField("ComponentControllerCategory") { FieldType = CSharpPrimitiveType.String(), Modifiers = CSharpModifiers.Const, Visibility = CSharpVisibility.Public, InitValue = macro.Value });

        var namespaces = new List<CppNamespace>();
        foreach (var ns in cppAst.Namespaces)
        {
            ProcessNamespaces(ns, namespaces);
        }

        foreach (var ns in namespaces)
        {
            ProcessConstFields(ns);

            foreach (var cppField in ns.Fields)
            {
                if (cppField.Name.EndsWith("_iid") && cppField.Type is CppQualifiedType qualifiedType && qualifiedType.ElementType is CppTypedef typedef && typedef.Name == "TUID")
                {
                    var fileContent = LoadContent(cppField.Span.Start.File);
                    var uuidText = fileContent[cppField.Span.Start.Line - 1];
                    var (uuidName, uuid) = ParseIID(uuidText);
                    _nameToIID.Add(uuidName, uuid);
                    if (_cppTypeToCSharpType.ContainsKey(uuidName))
                    {
                        GenerateGuid((CSharpStruct)_cppTypeToCSharpType[uuidName], uuid, uuidText);
                    }
                }
            }
        }

        GenerateComObjectManager();
        GenerateGuidToName();
    }

    private void GenerateComObjectManager()
    {
        // public sealed unsafe partial class ComObjectManager : IDisposable
        var comObjectManager = new CSharpClass("ComObjectManager");
        comObjectManager.Modifiers = CSharpModifiers.Partial;
        comObjectManager.Visibility = CSharpVisibility.Public;
        var staticMethod = new CSharpMethod()
        {
            Name = "RegisterAllInterfaces",
            ReturnType = CSharpPrimitiveType.Void(),
            Modifiers = CSharpModifiers.Static,
            Visibility = CSharpVisibility.Private,
        };
        comObjectManager.Members.Add(staticMethod);
        staticMethod.Body = (writer, element) =>
        {
            foreach (var item in _allGeneratedVtbls)
            {
                writer.WriteLine($"Register<{item.Name}>();");
            }
        };
        _container!.Members.Add(comObjectManager);
    }

    private void GenerateGuidToName()
    {
        // public sealed unsafe partial class ComObjectManager : IDisposable
        var staticMethod = new CSharpMethod()
        {
            Name = "GetMapGuidToName",
            ReturnType = new CSharpFreeType("Dictionary<Guid, string>"),
            Modifiers = CSharpModifiers.Static,
            Visibility = CSharpVisibility.Private,
        };
        _container!.Members.Add(staticMethod);

        staticMethod.Body = (writer, element) =>
        {
            writer.WriteLine("return new Dictionary<Guid, string>()");
            writer.OpenBraceBlock();
            foreach (var type in _cppTypeToCSharpType.Values.OfType<CSharpStructExtended>().Where(x => x.IsComObject).OrderBy(x => x.Name))
            {
                writer.WriteLine($"{{ {type.Name}.IId, nameof({type.Name}) }},");
            }
            writer.UnIndent();
            writer.WriteLine("};");
        };
    }


    private static string GetParentNamespace(CppNamespace ns)
    {
        var stack = new Stack<CppNamespace>();
        while (ns.Parent is CppNamespace parentNs)
        {
            stack.Push(parentNs);
            ns = parentNs;
        }

        var builder = new StringBuilder();
        bool isFirst = true;
        foreach (var parentNs in stack)
        {
            if (!isFirst) builder.Append(".");
            builder.Append(parentNs.Name);
            isFirst = false;
        }

        if (builder.Length > 0)
        {
            builder.Append(".");
        }

        return builder.ToString();
    }


    private void ProcessNamespaces(CppNamespace ns, List<CppNamespace> namespaces)
    {
        foreach (var cppClass in ns.Classes)
        {
            if (IsUnknown(cppClass) && !_typesToExclude.Contains(cppClass.Name))
            {
                GetOrCreateStruct(cppClass);
            }
            //else if (cppClass.Name.StartsWith("I"))
            //{
            //    //Console.WriteLine($"{GetParentNamespace(ns)}{cppClass.Name} not supported");
            //}
        }

        foreach (var cppEnum in ns.Enums)
        {
            var csEnum = ProcessEnum(cppEnum);
            if (csEnum != null)
            {
                _container!.Members.Add(csEnum);
            }
        }

        namespaces.Add(ns);
        foreach (var subNs in ns.Namespaces)
        {
            ProcessNamespaces(subNs, namespaces);
        }
    }


    private void ProcessConstFields(CppNamespace ns)
    {
        foreach (var cppField in ns.Fields)
        {
            if (cppField.InitValue?.Value is string text)
            {
                ProcessConstString(ns, cppField, text);
            }
            else if (cppField.Type is CppQualifiedType qualifiedType && qualifiedType.Qualifier == CppTypeQualifier.Const && qualifiedType.ElementType is CppTypedef typeDef)
            {
                // Process Speaker and SpeakerArrangement enum
                if (typeDef.Name == "Speaker" || typeDef.Name == "SpeakerArrangement")
                {
                    var csEnum = (CSharpEnum)_cppTypeToCSharpType["SpeakerArrangement"];
                    var csEnumItem = new CSharpEnumItem(cppField.Name, cppField.InitExpression.ToString()!.Replace("(Speaker)", string.Empty).Replace("1 <<", "1UL <<")) { CppElement = cppField };
                    csEnum.Members.Add(csEnumItem);
                }
            }
        }
    }

    private void ProcessConstString(CppNamespace ns, CppField cppField, string text)
    {
        CSharpClass? nsClass = null;
        bool isRootNameSpace = ns.Name == "Steinberg" || ns.Name == "Vst";
        var csField = new CSharpField(cppField.Name)
        {
            CppElement = cppField,
            FieldType = CSharpPrimitiveType.String(),
            Modifiers = CSharpModifiers.Const,
        };
        csField.InitValue = $"\"{text}\"";
        UpdateComment(csField);
        var csProperty = new CSharpProperty($"{cppField.Name}_u8")
        {
            CppElement = cppField,
            ReturnType = new CSharpFreeType("ReadOnlySpan<byte>"),
            Modifiers = CSharpModifiers.Static,
            Visibility = CSharpVisibility.Public,
            GetBodyInlined = $"\"{text}\\0\"u8"
        };
        UpdateComment(csProperty);

        CSharpTypeWithMembers container;

        if (isRootNameSpace)
        {
            container = _container!;
        }
        else
        {
            if (nsClass is null)
            {
                nsClass = new CSharpClass(ns.Name)
                {
                    Modifiers = CSharpModifiers.Static | CSharpModifiers.Partial,
                    Visibility = CSharpVisibility.Public
                };
                _container!.Members.Add(nsClass);
            }

            container = nsClass;
        }

        container.Members.Add(csField);
        container.Members.Add(csProperty);
    }

    private static readonly Regex MatchDashes = new(@"^-+(?:\r?\n|$)", RegexOptions.Multiline);

    private void UpdateComment(CSharpElement csElement)
    {
        if (csElement is ICSharpWithComment commentable && csElement.CppElement is not null)
        {
            var comment = DefaultCommentConverter.ConvertComment(null, csElement.CppElement, null);
            if (comment != null && comment.Children.Count > 0)
            {
                if (comment.Children[0].Children.Count > 0 && comment.Children[0].Children[0] is CSharpTextComment text)
                {
                    if (MatchDashes.Match(text.Text).Success)
                    {
                        text.Text = MatchDashes.Replace(text.Text, string.Empty);
                    }
                }
            }
            commentable.Comment = comment;
        }
    }

    private CSharpEnum? ProcessEnum(CppEnum cppEnum)
    {
        if (string.IsNullOrEmpty(cppEnum.Name)) return null;

        if (cppEnum.Name.StartsWith("(unnamed"))
        {
            cppEnum.Name = Path.GetFileNameWithoutExtension(cppEnum.SourceFile);
        }

        if (_typesToExclude.Contains(cppEnum.Name)) return null;

        var csEnum = new CSharpEnum(cppEnum.Name)
        {
            CppElement = cppEnum
        };
        
        bool isSigned = cppEnum.IntegerType.Equals(CppPrimitiveType.Int) ||
                        cppEnum.IntegerType.Equals(CppPrimitiveType.Short) ||
                        cppEnum.IntegerType.Equals(CppPrimitiveType.Char);

        UpdateComment(csEnum);

        if (!cppEnum.IntegerType.Equals(CppPrimitiveType.Int))
        {
            csEnum.BaseTypes.Add(GetCSharpType(cppEnum.IntegerType));
        }

        foreach (var cppEnumItem in cppEnum.Items)
        {
            var enumValue = cppEnumItem.ValueExpression?.ToString();
            if (enumValue == "0xFFFFFFFF" && isSigned)
            {
                enumValue = "-1";
            }
            var csEnumItem = new CSharpEnumItem(cppEnumItem.Name, enumValue) {CppElement = cppEnumItem };
            UpdateComment(csEnumItem);
            csEnum.Members.Add(csEnumItem);
        }

        return csEnum;
    }


    private CSharpType GetCSharpType(CppType cppType)
    {
        var csType = GetCSharpTypeImpl(cppType);
        csType.CppElement = cppType;
        return csType;
    }

    private CSharpType GetCSharpTypeImpl(CppType cppType)
    {
        if (cppType is CppQualifiedType qualifiedType && qualifiedType.Qualifier == CppTypeQualifier.Const)
        {
            cppType = qualifiedType.ElementType;
        }

        if (cppType is CppPointerType pointerType)
        {
            return new CSharpPointerType(GetCSharpType(pointerType.ElementType));
        }

        if (cppType is CppReferenceType refType)
        {
            return new CSharpPointerType(GetCSharpType(refType.ElementType));
        }

        if (cppType is CppEnum cppEnum)
        {
            return ProcessEnum(cppEnum)!;
        }

        var typeName = cppType.GetDisplayName();
        switch (typeName)
        {
            case "bool":
            case "TBool":
                return CSharpPrimitiveType.Byte();

            case "int8":
                return CSharpPrimitiveType.SByte();
            case "unsigned char":
            case "char8":
            case "char":
            case "uint8":
                return CSharpPrimitiveType.Byte();
            case "tchar":
            case "TChar":
            case "char16":
                return CSharpPrimitiveType.Char();
            case "int16": return CSharpPrimitiveType.Short();
            case "uint16": return CSharpPrimitiveType.UShort();
            case "int32": return CSharpPrimitiveType.Int();
            case "uint32": return CSharpPrimitiveType.UInt();
            case "int64": return CSharpPrimitiveType.Long();
            case "uint64": return CSharpPrimitiveType.ULong();
            case "void": return CSharpPrimitiveType.Void();
            case "TUID": return new CSharpFreeType("Guid");
            case "float": return CSharpPrimitiveType.Float();
            case "double": return CSharpPrimitiveType.Double();
            case "unsigned int": return CSharpPrimitiveType.UInt();
            //case "short": return CSharpPrimitiveType.Short();
            //case "unsigned short": return CSharpPrimitiveType.UShort();
            case "tresult": return new CSharpFreeType("ComResult");
            default:
                if (!_cppTypeToCSharpType.TryGetValue(cppType.GetDisplayName(), out var csType))
                {
                    if (cppType is CppTypedef typeDef)
                    {
                        if (typeDef.ElementType is CppArrayType arrayType)
                        {
                            var csStruct = new CSharpStructExtended(FilterName(typeDef.Name, false)) { IsFixedArray = true };
                            var csField = new CSharpField("Value")
                            {
                                FieldType = new CSharpFixedArrayType(GetCSharpType(arrayType.ElementType), arrayType.Size)
                            };
                            csStruct.Members.Add(csField);
                            csType = csStruct;
                            ApplyUnsafe(csStruct, csField.FieldType);
                        }
                        else
                        {
                            if (typeDef.Name == "SpeakerArrangement")
                            {
                                var csEnum = new CSharpEnum("SpeakerArrangement")
                                {
                                    BaseTypes = { CSharpPrimitiveType.ULong() },
                                    CppElement = typeDef
                                };
                                UpdateComment(csEnum);
                                csType = csEnum;
                            }
                            else
                            {
                                var csStruct = new CSharpStruct(FilterName(typeDef.Name, false));
                                var csParameterType = GetCSharpType(typeDef.ElementType);
                                csType = csStruct;
                                if (csParameterType is CSharpPointerType)
                                {
                                    csStruct.Members.Add(new CSharpField("Value") { FieldType = csParameterType });
                                    csStruct.Members.Add(new CSharpFreeMember() { Text = $"public static implicit operator {csParameterType.GetName()}({csStruct.Name} value) => value.Value;" });
                                    csStruct.Members.Add(new CSharpFreeMember() { Text = $"public static implicit operator {csStruct.Name}({csParameterType.GetName()} value) => new {csStruct.Name}() {{ Value = value }};" });
                                }
                                else
                                {
                                    csStruct.IsRecord = true;
                                    csStruct.RecordParameters.Add(new CSharpParameter("Value") { ParameterType = csParameterType });
                                    csStruct.Members.Add(new CSharpFreeMember() { Text = $"public static implicit operator {csParameterType.GetName()}({csStruct.Name} value) => value.Value;" });
                                    csStruct.Members.Add(new CSharpFreeMember() { Text = $"public static implicit operator {csStruct.Name}({csParameterType.GetName()} value) => new(value);" });

                                }

                                ApplyUnsafe(csStruct, csParameterType);
                            }
                        }
                        _cppTypeToCSharpType[typeDef.Name] = csType;
                        _container!.Members.Add(csType);
                    }
                    else if (cppType is CppClass cppClass)
                    {
                        var csStruct = GetOrCreateStruct(cppClass);
                        csType = csStruct;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Not supported type {cppType.GetType().FullName} / {cppType}");
                    }

                }

                return csType;
        }
    }

    private CSharpType GetCSharpReturnOrParameterType(CppType cppType)
    {
        var type = GetCSharpType(cppType);
        // If the parameter is fixed array, pass a pointer
        if (type is CSharpStructExtended { IsFixedArray: true })
        {
            type = new CSharpPointerType(type);
        }

        if (type is CSharpFreeType freeType && freeType.Text == "Guid")
        {
            type = new CSharpPointerType(type);
        }

        // We don't support char
        if (type is CSharpPrimitiveType primitive && primitive.Kind == CSharpPrimitiveKind.Char)
        {
            type = CSharpPrimitiveType.UShort();
        }

        return type;
    }
    private CSharpType GetUnmanagedType(CSharpType csType)
    {
        if (csType is CSharpStruct csStruct)
        {
            var name = csStruct.Name;
            if (csType.CppElement is CppTypedef typeDef)
            {
                var canonicalType = typeDef.ElementType.GetCanonicalType();
                if (canonicalType.TypeKind == CppTypeKind.Primitive || canonicalType.TypeKind == CppTypeKind.Pointer)
                {
                    return GetCSharpType(typeDef.ElementType);
                }
            }
        }
        else if (csType.ToString() == "ComResult")
        {
            return CSharpPrimitiveType.Int();
        }

        return csType;
    }

    static void ApplyUnsafe(CSharpStruct csStruct, CSharpType csType)
    {
        if (csType is CSharpPointerType || csType is CSharpFixedArrayType)
        {
            csStruct.Modifiers |= CSharpModifiers.Unsafe;
        }
    }

    private static string FilterName(string name, bool isParameter)
    {
        if (name == "string") return "@string";
        if (name == "event") return "@event";
        if (name == "object") return "@object";
        if (string.IsNullOrEmpty(name)) return isParameter ? "arg" : "union";
        return name;
    }

    [Flags]
    private enum InterfaceKind
    {
        None = 0,
        Plugin = 1 << 0,
        Host = 1 << 1,
        Both = Plugin | Host
    }

    private CSharpStructExtended GetOrCreateStruct(CppClass cppClass, CSharpTypeWithMembers? container = null)
    {
        var name = cppClass.Name;
        bool isUnion = cppClass.ClassKind == CppClassKind.Union;
        bool isAnonymousUnion = isUnion && string.IsNullOrEmpty(name);
        if (isAnonymousUnion)
        {
            name = "Union";
        }

        if (container is null && cppClass.Parent is CppClass parentCpp)
        {
            container = (CSharpStruct)GetCSharpType(parentCpp);
        }

        if (_cppTypeToCSharpType.TryGetValue(name, out var csType))
        {
            return (CSharpStructExtended)csType;
        }

        var csStruct = new CSharpStructExtended(name)
        {
            Modifiers = CSharpModifiers.Unsafe | CSharpModifiers.Partial,
            Visibility = CSharpVisibility.Public,
            CppElement = cppClass,
        };

        if (!isAnonymousUnion)
        {
            _cppTypeToCSharpType[name] = csStruct;
        }

        container ??= _container!;
        container.Members.Add(csStruct);
        UpdateComment(csStruct);

        if (cppClass.Functions.All(x => (x.Flags & CppFunctionFlags.Pure) == 0))
        {
            csStruct.Attributes.Add(isUnion
                ? new CSharpStructLayoutAttribute(LayoutKind.Explicit) { CharSet = CharSet.Unicode }
                : new CSharpStructLayoutAttribute(LayoutKind.Sequential) { CharSet = CharSet.Unicode, Pack = 16 }
            );

            foreach (var cppField in cppClass.Fields)
            {
                if (cppField.StorageQualifier == CppStorageQualifier.None)
                {
                    var csField = new CSharpField(FilterName(cppField.Name, false)) { CppElement = cppField };
                    UpdateComment(csField);
                    if (isUnion)
                    {
                        csField.Attributes.Add(new CSharpFreeAttribute("FieldOffset(0)"));
                    }
                    if (cppField.Type is CppArrayType arrayType)
                    {
                        csField.FieldType = new CSharpFixedArrayType(GetCSharpType(arrayType.ElementType), arrayType.Size);
                    }
                    else
                    {
                        csField.FieldType = GetCSharpType(cppField.Type);
                    }
                    csStruct.Members.Add(csField);
                }
            }
        }
        else
        {
            // Process base classes
            foreach (var cppClassBaseType in cppClass.BaseTypes)
            {
                if (cppClassBaseType.Type is CppClass cppClassBase)
                {
                    var comStruct = GetOrCreateStruct(cppClassBase);
                    if (comStruct.ComMethodCount > 0)
                    {
                        csStruct.BaseComMethodIndex = comStruct.BaseComMethodIndex + comStruct.ComMethodCount;
                        break;
                    }
                }
            }

            var kind = InterfaceKind.Both;
            csStruct.BaseTypes.Add(new CSharpFreeType("INativeGuid"));
            csStruct.IsComObject = true;
            csStruct.Members.Add(
                new CSharpProperty("NativeGuid")
                {
                    Modifiers = CSharpModifiers.Static,
                    Visibility = CSharpVisibility.Public,
                    ReturnType = new CSharpPointerType(new CSharpFreeType("Guid")),
                    GetBodyInlined = "(Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in IId))"
                }
            );

            CSharpMethod? initializeVtbl = null;
            CSharpProperty? vtblCount = null;

            var voidPtrPtr = new CSharpPointerType(new CSharpPointerType(CSharpPrimitiveType.Void()));

            var rcwMethods = new List<CSharpMethod>();
            var toManagedMethods = new List<CSharpMethod>();
            var wrapperMethods = new List<CSharpMethod>();
            int virtualMethodIndex = 0;

            if ((kind & InterfaceKind.Host) != 0)
            {
                csStruct.Members.Add(new CSharpField("Vtbl") { FieldType = voidPtrPtr });
                var baseType = GetBaseType(cppClass);
                if (baseType is not null)
                {
                    WriteBaseMethods(baseType, csStruct, rcwMethods, ref virtualMethodIndex, (CSharpStruct)GetCSharpType(baseType));
                }
            }

            CSharpStruct? ccwStructImpl = null;

            foreach (var cppMethod in cppClass.Functions)
            {
                if ((cppMethod.Flags & CppFunctionFlags.Pure) == 0) continue;

                if ((kind & InterfaceKind.Host) != 0)
                {
                    var csMethod = CreateMethodRcw(cppMethod, csStruct, virtualMethodIndex);
                    rcwMethods.Add(csMethod);
                    if (!csStruct.BaseTypes.Any(x => x is CSharpFreeType freeType && freeType.Text == "INativeUnknown"))
                    {
                        csStruct.BaseTypes.Add(new CSharpFreeType("INativeUnknown"));
                    }
                }

                if ((kind & InterfaceKind.Plugin) != 0)
                {
                    // Create vtbl
                    if (initializeVtbl is null)
                    {
                        csStruct.BaseTypes.Add(new CSharpFreeType("INativeVtbl"));

                        vtblCount = new CSharpProperty("VtblCount")
                        {
                            Modifiers = CSharpModifiers.Static,
                            Visibility = CSharpVisibility.Public,
                            ReturnType = CSharpPrimitiveType.Int(),
                        };
                        csStruct.Members.Add(vtblCount);

                        initializeVtbl = new CSharpMethod() { Name = "InitializeVtbl", Modifiers = CSharpModifiers.Static };
                        initializeVtbl.Parameters.Add(new CSharpParameter("vtbl") { ParameterType = voidPtrPtr });
                        initializeVtbl.ReturnType = CSharpPrimitiveType.Void();
                        initializeVtbl.Attributes.Add(MethodImplAggressiveInliningAttribute);
                        csStruct.Members.Add(initializeVtbl);
                        _allGeneratedVtbls.Add(csStruct);

                        var ccwFile = $"LibVst.{name}.cs";
                        if (!File.Exists(Path.Combine(_destinationFolder, ccwFile)))
                        {
                            var csFile = new CSharpGeneratedFile($"/{ccwFile}")
                            {
                                EmitAutoGenerated = false
                            };
                            csFile.Members.Add(new CSharpSimpleComment() { Children = { new CSharpTextComment("Copyright (c) Alexandre Mutel. All rights reserved.") } });
                            csFile.Members.Add(new CSharpSimpleComment() { Children = { new CSharpTextComment("Licensed under the BSD-Clause 2 license.") } });
                            csFile.Members.Add(new CSharpSimpleComment() { Children = { new CSharpTextComment("See license.txt file in the project root for full license information.") } });
                            csFile.Members.Add(new CSharpNamespace("NPlug.Interop") { IsFileScoped = true });
                            csFile.Members.Add(new CSharpUsingDeclaration("System"));

                            var csLibVst = new CSharpClass("LibVst")
                            {
                                Modifiers = CSharpModifiers.Static | CSharpModifiers.Partial | CSharpModifiers.Unsafe,
                                Visibility = CSharpVisibility.Internal
                            };
                            csFile.Members.Add(csLibVst);

                            ccwStructImpl = new CSharpStruct(name)
                            {
                                Modifiers = CSharpModifiers.Partial,
                                Visibility = CSharpVisibility.Public,
                            };
                            csLibVst.Members.Add(ccwStructImpl);

                            _toManagedFiles.Add(csFile);
                        }
                    }

                    var csMethod = new CSharpMethod
                    {
                        Name = $"{cppMethod.Name}_ToManaged",
                        Modifiers = CSharpModifiers.Static | CSharpModifiers.Partial,
                        Visibility = CSharpVisibility.Private,
                        CppElement = cppMethod,
                    };
                    UpdateComment(csMethod);

                    var csMethodWrap = new CSharpMethod
                    {
                        Name = $"{cppMethod.Name}_Wrapper",
                        Modifiers = CSharpModifiers.Static,
                        Visibility = CSharpVisibility.Private,
                        CppElement = cppMethod,
                    };
                    csMethodWrap.Attributes.Add(new CSharpFreeAttribute("UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction)})"));

                    csMethod.ReturnType = GetCSharpReturnOrParameterType(cppMethod.ReturnType);
                    csMethodWrap.ReturnType = GetUnmanagedType(csMethod.ReturnType);
                    csMethod.Parameters.Add(new CSharpParameter("self") { ParameterType = new CSharpFreeType($"{name}*") });
                    foreach (var cppMethodParameter in cppMethod.Parameters)
                    {
                        var csParameter = new CSharpParameter
                        {
                            Name = FilterName(cppMethodParameter.Name, true),
                            ParameterType = GetCSharpReturnOrParameterType(cppMethodParameter.Type)
                        };
                        csMethod.Parameters.Add(csParameter);
                    }

                    // Copy C# parameter for wrapper, but replace any typedef to parameter
                    foreach (var csParameter in csMethod.Parameters)
                    {
                        var csParameterForWrapper = new CSharpParameter()
                        {
                            Name = csParameter.Name,
                            ParameterType = GetUnmanagedType(csParameter.ParameterType)
                        };
                        csMethodWrap.Parameters.Add(csParameterForWrapper);
                    }

                    csMethodWrap.Body = (writer, element) =>
                    {
                        var returnTypeAsStr = csMethod.ReturnType.ToString();
                        var isComResult = returnTypeAsStr == "ComResult";

                        writer.WriteLine("if (InteropHelper.IsTracerEnabled)");
                        writer.OpenBraceBlock();
                        {
                            writer.WriteLine($"var __evt__ = new NativeToManagedEvent((IntPtr)self, nameof({csStruct.Name}), \"{cppMethod.Name}\");");
                            writer.WriteLine("try");
                            writer.OpenBraceBlock();

                            if (returnTypeAsStr != "void")
                            {
                                writer.Write("return ");
                            }

                            writer.Write(csMethod.Name);
                            writer.Write("(");
                            for (var i = 0; i < csMethod.Parameters.Count; i++)
                            {
                                var csParameter = csMethod.Parameters[i];
                                if (i > 0)
                                {
                                    writer.Write(", ");
                                }
                                writer.Write(csParameter.Name);
                            }
                            writer.WriteLine(");");

                            writer.CloseBraceBlock();
                            writer.WriteLine("catch (Exception ex)");
                            writer.OpenBraceBlock();
                            writer.WriteLine("__evt__.Exception = ex;");
                            if (isComResult)
                            {
                                writer.WriteLine("return (ComResult)ex;");
                            }
                            else if (returnTypeAsStr != "void")
                            {
                                writer.WriteLine("return default;");
                            }
                            writer.CloseBraceBlock();
                            writer.WriteLine("finally");
                            writer.OpenBraceBlock();
                            writer.WriteLine("__evt__.Dispose();");
                            writer.CloseBraceBlock();
                        }
                        writer.CloseBraceBlock();
                        writer.WriteLine("else");
                        writer.OpenBraceBlock();
                        {
                            writer.WriteLine("try");
                            writer.OpenBraceBlock();

                            if (returnTypeAsStr != "void")
                            {
                                writer.Write("return ");
                            }

                            writer.Write(csMethod.Name);
                            writer.Write("(");
                            for (var i = 0; i < csMethod.Parameters.Count; i++)
                            {
                                var csParameter = csMethod.Parameters[i];
                                if (i > 0)
                                {
                                    writer.Write(", ");
                                }
                                writer.Write(csParameter.Name);
                            }
                            writer.WriteLine(");");

                            writer.CloseBraceBlock();
                            writer.WriteLine(isComResult ? "catch (Exception ex)" : "catch");
                            writer.OpenBraceBlock();
                            if (isComResult)
                            {
                                writer.WriteLine("return (ComResult)ex;");
                            }
                            else if (returnTypeAsStr != "void")
                            {
                                writer.WriteLine("return default;");
                            }
                            writer.CloseBraceBlock();
                        }
                        writer.CloseBraceBlock();
                    };

                    if (ccwStructImpl is not null)
                    {
                        var csMethodImpl = new CSharpMethod
                        {
                            Name = $"{cppMethod.Name}_ToManaged",
                            Modifiers = CSharpModifiers.Static | CSharpModifiers.Partial,
                            Visibility = CSharpVisibility.Private,
                            Body = (writer, element) =>
                            {
                                writer.WriteLine("throw new NotImplementedException();");
                            }
                        };
                        csMethodImpl.ReturnType = csMethod.ReturnType;
                        csMethodImpl.Parameters.AddRange(csMethod.Parameters);
                        ccwStructImpl.Members.Add(csMethodImpl);
                    }

                    toManagedMethods.Add(csMethod);
                    wrapperMethods.Add(csMethodWrap);
                }

                virtualMethodIndex++;
            }

            if (rcwMethods.Count > 0)
            {
                csStruct.Members.Add(new CSharpSimpleComment() { Children = { new CSharpTextComment("--------------------------------------------------------------") } });
                csStruct.Members.Add(new CSharpSimpleComment() { Children = { new CSharpTextComment("RCW methods") } });
                csStruct.Members.Add(new CSharpSimpleComment() { Children = { new CSharpTextComment("--------------------------------------------------------------") } });

                foreach (var cSharpMethod in rcwMethods)
                {
                    csStruct.Members.Add(cSharpMethod);
                }
            }

            if (initializeVtbl != null && vtblCount != null)
            {
                var baseType = GetBaseType(cppClass);
                int baseComMethodIndex = 0;
                if (baseType != null)
                {
                    var csBaseType = GetOrCreateStruct(baseType);
                    baseComMethodIndex = csBaseType.BaseComMethodIndex + csBaseType.ComMethodCount;
                }

                csStruct.ComMethodCount = wrapperMethods.Count;

                var totalMethodCount = baseComMethodIndex + csStruct.ComMethodCount;

                vtblCount.GetBodyInlined = $"{totalMethodCount}";

                csStruct.Members.Add(new CSharpSimpleComment() { Children = { new CSharpTextComment("--------------------------------------------------------------") } });
                csStruct.Members.Add(new CSharpSimpleComment() { Children = { new CSharpTextComment("CCW methods") } });
                csStruct.Members.Add(new CSharpSimpleComment() { Children = { new CSharpTextComment("--------------------------------------------------------------") } });
                initializeVtbl.Body = (writer, element) =>
                {
                    int vtblIndex = baseComMethodIndex;
                    if (baseType != null)
                    {
                        writer.WriteLine($"{baseType.Name}.InitializeVtbl(vtbl);");
                    }

                    foreach (var method in wrapperMethods)
                    {
                        var functionPointer = method.ToFunctionPointer();
                        functionPointer.IsUnmanaged = true;
                        functionPointer.UnmanagedCallingConvention.Add("MemberFunction");
                        writer.WriteLine($"vtbl[{vtblIndex}] = ({functionPointer})&{method.Name};");
                        vtblIndex++;
                    }
                };

                for (var i = 0; i < wrapperMethods.Count; i++)
                {
                    var toManagedMethod = toManagedMethods[i];
                    var wrapperMethod = wrapperMethods[i];
                    csStruct.Members.Add(toManagedMethod);
                    csStruct.Members.Add(wrapperMethod);
                }

            }
        }

        foreach (var cppEnum in cppClass.Enums)
        {
            var csEnum = ProcessEnum(cppEnum);
            if (csEnum != null)
            {
                csStruct.Members.Add(csEnum);
            }
        }

        return csStruct;
    }

    private static CppClass? GetBaseType(CppClass cppClass) => cppClass.BaseTypes.Count > 0 ? cppClass.BaseTypes[0].Type as CppClass : null;

    private void WriteBaseMethods(CppClass cppClass, CSharpStruct csClass, List<CSharpMethod> rcwMethods, ref int virtualMethodIndex, CSharpStruct? csBaseClass = null)
    {
        var baseClass = cppClass.BaseTypes.Count > 0 ? cppClass.BaseTypes[0].Type as CppClass : null;
        if (baseClass is not null)
        {
            WriteBaseMethods(baseClass, csClass, rcwMethods, ref virtualMethodIndex, (CSharpStruct)GetCSharpType(baseClass));
        }

        foreach (var cppMethod in cppClass.Functions.Where(function => (function.Flags & CppFunctionFlags.Pure) != 0))
        {
            var csMethod = CreateMethodRcw(cppMethod, csClass, virtualMethodIndex, csBaseClass);
            rcwMethods.Add(csMethod);
            virtualMethodIndex++;
        }
    }

    private static readonly CSharpAttribute MethodImplAggressiveInliningAttribute = new CSharpFreeAttribute($"MethodImpl({nameof(MethodImplOptions)}.{nameof(MethodImplOptions.AggressiveInlining)})");

    private CSharpMethod CreateMethodRcw(CppFunction cppMethod, CSharpStruct csClass, int virtualMethodIndex, CSharpStruct? csBaseClass = null)
    {
        var csMethod = new CSharpMethod
        {
            Name = cppMethod.Name,
            Visibility = CSharpVisibility.Public,
            CppElement = cppMethod,
        };
        UpdateComment(csMethod);
        csMethod.Attributes.Add(MethodImplAggressiveInliningAttribute);
        if (csBaseClass != null)
        {
            //csMethod.Attributes.Add(new CSharpFreeAttribute("UnscopedRef"));
        }

        csMethod.ReturnType = GetCSharpReturnOrParameterType(cppMethod.ReturnType);
        var unmanagedReturnType = GetUnmanagedType(csMethod.ReturnType);
        if (csMethod.ReturnType.ToString() != "ComResult")
        {
            csMethod.ReturnType = unmanagedReturnType;
        }

        foreach (var cppMethodParameter in cppMethod.Parameters)
        {
            var csParameter = new CSharpParameter
            {
                Name = FilterName(cppMethodParameter.Name, true),
                ParameterType = GetCSharpReturnOrParameterType(cppMethodParameter.Type)
            };
            csMethod.Parameters.Add(csParameter);
        }

        var localVirtualMethodIndex = virtualMethodIndex;

        if (csBaseClass != null)
        {
            csMethod.Body = (writer, element) =>
            {
                var builder = new StringBuilder();
                if (!cppMethod.ReturnType.Equals(CppPrimitiveType.Void))
                {
                    builder.Append("return ");
                }
                builder.Append($"Unsafe.As<{csClass.Name}, {csBaseClass.Name}>(ref this).{csMethod.Name}(");
                for (var i = 0; i < csMethod.Parameters.Count; i++)
                {
                    var csParam = csMethod.Parameters[i];
                    if (i > 0) builder.Append(", ");
                    builder.Append(csParam.Name);
                }

                builder.Append(");");
                writer.WriteLine(builder.ToString());
            };
        }
        else
        {
            csMethod.Body = (writer, element) =>
            {
                var functionPointer = csMethod.ToFunctionPointer();
                var thisPointer = new CSharpPointerType(csClass);
                functionPointer.IsUnmanaged = true;
                functionPointer.UnmanagedCallingConvention.Add("MemberFunction");
                functionPointer.Parameters.Insert(0, thisPointer);
                functionPointer.ReturnType = unmanagedReturnType;
                var builder = new StringBuilder();

                writer.WriteLine("if (InteropHelper.IsTracerEnabled)");
                writer.OpenBraceBlock();
                {
                    writer.WriteLine($"var __self__ = ({thisPointer})Unsafe.AsPointer(ref this);");
                    writer.WriteLine($"var __evt__ = new ManagedToNativeEvent((IntPtr)__self__, nameof({csClass.Name}), \"{cppMethod.Name}\");");

                    if (!cppMethod.ReturnType.Equals(CppPrimitiveType.Void))
                    {
                        builder.Append("var __result__ = ");
                    }
                    builder.Append($"(({functionPointer})Vtbl[{localVirtualMethodIndex}])(__self__");
                    for (var i = 0; i < csMethod.Parameters.Count; i++)
                    {
                        var csParam = csMethod.Parameters[i];
                        builder.Append(", ");
                        builder.Append(csParam.Name);
                    }

                    builder.Append(");");
                    writer.WriteLine(builder.ToString());

                    if (csMethod.ReturnType.ToString() == "ComResult")
                    {
                        writer.WriteLine("__evt__.Result = __result__;");
                    }
                    writer.WriteLine("__evt__.Dispose();");
                    if (!cppMethod.ReturnType.Equals(CppPrimitiveType.Void))
                    {
                        writer.WriteLine("return __result__;");
                    }
                }
                writer.CloseBraceBlock();
                writer.WriteLine("else");
                writer.OpenBraceBlock();
                {
                    builder.Clear();
                    if (!cppMethod.ReturnType.Equals(CppPrimitiveType.Void))
                    {
                        builder.Append("return ");
                    }
                    builder.Append($"(({functionPointer})Vtbl[{localVirtualMethodIndex}])(({thisPointer})Unsafe.AsPointer(ref this)");
                    for (var i = 0; i < csMethod.Parameters.Count; i++)
                    {
                        var csParam = csMethod.Parameters[i];
                        builder.Append(", ");
                        builder.Append(csParam.Name);
                    }

                    builder.Append(");");
                    writer.WriteLine(builder.ToString());
                }
                writer.CloseBraceBlock();
            };
        }
        return csMethod;
    }

    private void GenerateGuid(CSharpStruct csStruct, Uuid uuid, string comment)
    {
        var prop = new CSharpProperty("IId")
        {
            Modifiers = CSharpModifiers.Static,
            ReturnType = new CSharpRefType(CSharpRefKind.RefReadOnly, new CSharpFreeType("Guid")),
            Visibility = CSharpVisibility.Public,
            //prop.GetAttributes(),
            AttributesForGet =
            {
                MethodImplAggressiveInliningAttribute
            },
            GetBody = (writer, element) =>
            {
                var builder = new StringBuilder();
                builder.AppendLine("return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference((OperatingSystem.IsWindows()");
                builder.Append("        ? new ReadOnlySpan<byte>(new byte[] { ");
                builder.Append(uuid.ToBytes(true));
                builder.AppendLine(" })");
                builder.Append("        : new ReadOnlySpan<byte>(new byte[] { ");
                builder.Append(uuid.ToBytes(false));
                builder.AppendLine(" })");
                builder.Append("    )));");
                writer.WriteLine(builder.ToString());
            }
        };
        prop.Comment = new CSharpFullComment()
        {
            Children =
            {
                new CSharpXmlComment("summary")
                {
                    Children =
                    {
                        new CSharpTextComment(comment)
                    }
                }
            }
        };

        csStruct.Members.Add(prop);
    }

    private static readonly Regex RegexMatchIID = new Regex(@"\w+\s*\((\w+)\s*,\s*(0x\w+)\s*,\s*(0x\w+)\s*,\s*(0x\w+)\s*,\s*(0x\w+)\s*\)");

    private static (string, Uuid) ParseIID(string uuidText)
    {
        // DECLARE_CLASS_IID (IComponent, 0xE831FF31, 0xF2D54301, 0x928EBBEE, 0x25697802)
        var match = RegexMatchIID.Match(uuidText);
        if (!match.Success)
        {
            throw new InvalidOperationException($"Unexpected UUID declaration: {uuidText}");
        }

        var result = new Uuid()
        {
            Value1 = Convert.ToUInt32(match.Groups[2].Value, 16),
            Value2 = Convert.ToUInt32(match.Groups[3].Value, 16),
            Value3 = Convert.ToUInt32(match.Groups[4].Value, 16),
            Value4 = Convert.ToUInt32(match.Groups[5].Value, 16),
        };

        return (match.Groups[1].Value, result);
    }

    private string[] LoadContent(string filePath)
    {
        if (!_fileToContent.TryGetValue(filePath, out var content))
        {
            content = File.ReadLines(filePath).ToArray();
            _fileToContent[filePath] = content;
        }
        return content;
    }

    record struct Uuid(uint Value1, uint Value2, uint Value3, uint Value4)
    {
        public ushort Value2High => (ushort)(Value2 >> 16);

        public ushort Value2Low => (ushort)Value2;

        public override string ToString()
        {
            return $"0x{Value1:x8}, 0x{Value2:x8}, 0x{Value3:x8}, 0x{Value4:x8}";
        }

        public string ToBytes(bool isWindows)
        {
            return isWindows
                    ? $"0x{Value1 & 0xFF:x2}, 0x{(Value1 >> 8) & 0xFF:x2}, 0x{(Value1 >> 16) & 0xFF:x2}, 0x{(Value1 >> 24) & 0xFF:x2}, " +
                      $"0x{(Value2 >> 16) & 0xFF:x2}, 0x{(Value2 >> 24) & 0xFF:x2}, 0x{Value2 & 0xFF:x2}, 0x{(Value2 >> 8) & 0xFF:x2}, " +
                      $"0x{(Value3 >> 24) & 0xFF:x2}, 0x{(Value3 >> 16) & 0xFF:x2}, 0x{(Value3 >> 8) & 0xFF:x2}, 0x{Value3 & 0xFF:x2}, " +
                      $"0x{(Value4 >> 24) & 0xFF:x2}, 0x{(Value4 >> 16) & 0xFF:x2}, 0x{(Value4 >> 8) & 0xFF:x2}, 0x{Value4 & 0xFF:x2}"
                    : $"0x{(Value1 >> 24) & 0xFF:x2}, 0x{(Value1 >> 16) & 0xFF:x2}, 0x{(Value1 >> 8) & 0xFF:x2}, 0x{Value1 & 0xFF:x2}, " +
                      $"0x{(Value2 >> 24) & 0xFF:x2}, 0x{(Value2 >> 16) & 0xFF:x2}, 0x{(Value2 >> 8) & 0xFF:x2}, 0x{Value2 & 0xFF:x2}, " +
                      $"0x{(Value3 >> 24) & 0xFF:x2}, 0x{(Value3 >> 16) & 0xFF:x2}, 0x{(Value3 >> 8) & 0xFF:x2}, 0x{Value3 & 0xFF:x2}, " +
                      $"0x{(Value4 >> 24) & 0xFF:x2}, 0x{(Value4 >> 16) & 0xFF:x2}, 0x{(Value4 >> 8) & 0xFF:x2}, 0x{Value4 & 0xFF:x2}"
                ;
        }
    }

    private class CSharpStructExtended : CSharpStruct
    {
        public CSharpStructExtended(string name) : base(name)
        {
        }

        public int BaseComMethodIndex { get; set; }

        public int ComMethodCount { get; set; }

        public bool IsComObject { get; set; }

        public bool IsFixedArray { get; set; }
    }
}