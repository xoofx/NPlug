using CppAst;

namespace NPlug.CodeGen;

internal class Program
{
    static void Main(string[] args)
    {
        // src\NPlug.CodeGen\bin\Debug\net7.0\win-x64\
        var rootFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".."));
        var sdkFolder = Path.Combine(rootFolder, "ext", "vst3sdk");
        if (!Directory.Exists(sdkFolder))
        {
            throw new DirectoryNotFoundException($"The sdk folder {sdkFolder} was not found. Run ext/nplub_validator/build_nplug_validator.ps1 before running this program.");
        }

        var codeGenerator = new CodeGenerator(sdkFolder);
        codeGenerator.Generate(Path.Combine(rootFolder, "src", "NPlug", "Interop"));
    }
}