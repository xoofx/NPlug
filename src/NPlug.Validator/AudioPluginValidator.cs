using System.Reflection;
using NPlug.Proxy;
using System.Runtime.InteropServices;

namespace NPlug.Validator;

public static class AudioPluginValidator
{
    private static readonly AudioPluginProxy NativeProxy;

    public const string DefaultPluginName = "nplug_validator_proxy";

    public static string DefaultPluginPath => Path.Combine(AppContext.BaseDirectory, $"{DefaultPluginName}.vst3");

    static AudioPluginValidator()
    {
        Initialize();
        NativeProxy = AudioPluginProxy.Load(Path.Combine(DefaultPluginPath, "Contents", AudioPluginProxy.GetVstArchitecture(), AudioPluginProxy.GetVstDynamicLibraryName(DefaultPluginName)));
    }

    public static bool Validate(AudioPluginFactory factory, TextWriter outputLog, TextWriter errorLog)
    {
        NativeProxy.SetNativeFactory(factory.Export);

        return Validate(2, new string[]
        {
            nameof(AudioPluginValidator),
            DefaultPluginPath
        }, outputLog, errorLog) == 0;
    }

    public static bool Validate(string pluginPath, TextWriter outputLog, TextWriter errorLog)
    {
        return Validate(2, new string[]
        {
            nameof(AudioPluginValidator),
            pluginPath
        }, outputLog, errorLog) == 0;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void FunctionOutputDelegate(int c);

    private static int Validate(int argc, string[] args, TextWriter outputLog, TextWriter errorLog)
    {
        FunctionOutputDelegate outputLocalDelegate = c =>
        {
            outputLog.Write((char)c);
            outputLog.Flush();
        };
        FunctionOutputDelegate errorLocalDelegate = c =>
        {
            errorLog.Write((char)c);
            errorLog.Flush();
        };
        var outputLocalDelegatePtr = Marshal.GetFunctionPointerForDelegate(outputLocalDelegate);
        var outputLocalHandle = GCHandle.Alloc(outputLocalDelegate);

        var errorLocalDelegatePtr = Marshal.GetFunctionPointerForDelegate(errorLocalDelegate);
        var errorLocalHandle = GCHandle.Alloc(errorLocalDelegate);
        try
        {
            return Validate(argc, args, outputLocalDelegatePtr, errorLocalDelegatePtr);
        }
        finally
        {
            outputLocalHandle.Free();
            errorLocalHandle.Free();
        }
    }

    [DllImport("nplug_validator", EntryPoint = "nplug_validator_initialize")]
    private static extern void Initialize();

        
    [DllImport("nplug_validator", EntryPoint = "nplug_validator_validate")]
    private static extern int Validate(int argc, string[] argv, IntPtr output, IntPtr error);


    [DllImport("nplug_validator", EntryPoint = "nplug_validator_destroy")]
    private static extern void Destroy();
}