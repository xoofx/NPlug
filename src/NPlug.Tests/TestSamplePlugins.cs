using System.Text.RegularExpressions;
using NPlug.Interop;
using NPlug.SimpleDelay;
using NPlug.SimpleProgramChange;
using NPlug.Validator;

namespace NPlug.Tests;

public class TestSamplePlugins
{
    public static void Main()
    {
        // InteropHelper.Tracer = new TempFileInteropTracer();
        var factory = SimpleDelayPlugin.GetFactory();
        AudioPluginValidator.Validate(factory.Export, Console.Out, Console.Error);

        if (InteropHelper.HasObjectAlive())
        {
            Console.WriteLine("=============================================================");
            Console.WriteLine(InteropHelper.DumpObjectAlive());
        }
    }

    [Test]
    public Task TestSimpleDelay()
    {
        return VerifyPlugin(SimpleDelayPlugin.GetFactory);
    }

    [Test]
    public Task TestSimpleProgramChangePlugin()
    {
        return VerifyPlugin(SimpleProgramChangePlugin.GetFactory);
    }

    private Task VerifyPlugin(Func<AudioPluginFactory> factory)
    {
        var factoryInstance = factory();
        var outBuilder = new StringWriter();
        var errorBuilder = new StringWriter();
        var result = AudioPluginValidator.Validate(factoryInstance.Export, outBuilder, errorBuilder);
        var textOutput = outBuilder.ToString().Trim().Replace("\r\n", "\n");
        var errorOutput = errorBuilder.ToString().Trim();

        // Replace all nplug_validator_proxy.xxx by nplug_validator_proxy.vst3, as the extension change by platform
        textOutput = Regex.Replace(textOutput, $"{AudioPluginValidator.DefaultPluginName}.\\w+", $"{AudioPluginValidator.DefaultPluginName}.vst3");

        // Fix weird issue on MacOS where this warning is emitted 2 times
        const string CheckText = "Info:     Not all points have been read via IParameterChanges\n";
        var indexOf = textOutput.IndexOf(CheckText);
        if (indexOf > 0)
        {
            var newIndexOf = textOutput.IndexOf(CheckText, indexOf + 1);
            if (newIndexOf > 0 && newIndexOf == indexOf + CheckText.Length)
            {
                textOutput = textOutput.Substring(0, newIndexOf) + textOutput.Substring(newIndexOf + CheckText.Length);
            }
        }
        
        if (!string.IsNullOrEmpty(errorOutput))
        {
            textOutput += "\n*******************************************************\nError Output\n*******************************************************\n";
            textOutput += errorOutput;
        }

        if (!result)
        {
            TestContext.Out.WriteLine(textOutput);
        }
        Assert.True(result, "Error, the plugin failed, check the logs.");

        return Verify(textOutput, settings: GetVerifySettings());
    }

    private VerifySettings GetVerifySettings()
    {
        var settings = new VerifySettings();
        settings.UseDirectory("Snapshots");
        settings.DisableDiff();
        return settings;
    }
}
