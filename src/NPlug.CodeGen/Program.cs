using CppAst;

namespace NPlug.CodeGen;

internal class Program
{
    static void Main(string[] args)
    {
        var sdkFolder = @"C:\code\VST_SDK\vst3sdk";
        var codeGenerator = new CodeGenerator(sdkFolder);
        codeGenerator.Generate(@"C:\code\NPlug\src\NPlug\Vst3\");
    }
}