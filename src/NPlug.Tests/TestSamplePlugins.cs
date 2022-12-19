using NPlug.Interop;
using NPlug.SimpleDelay;
using NPlug.SimpleProgramChange;
using NPlug.Validator;

namespace NPlug.Tests;

public class TestSamplePlugins
{
    public static void Main()
    {
        //InteropHelper.Tracer = new InteropTracer();
        var factory = SimpleDelayPlugin.GetFactory();
        AudioPluginValidator.Validate(factory, Console.Out, Console.Error);

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
        var result = AudioPluginValidator.Validate(factoryInstance, outBuilder, errorBuilder);
        var textOutput = outBuilder.ToString().Trim();
        var errorOutput = errorBuilder.ToString().Trim();
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
