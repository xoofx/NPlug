using NPlug.IO;

namespace NPlug.HelloWorldPlugin;

public class HelloWorldProcessor : AudioProcessor
{
    private static readonly Guid ClassId = new("A1F15211-4DE0-4430-B1CE-8676927F3374");

    public HelloWorldProcessor() : base(AudioSampleSizeSupport.Any)
    {
    }
    
    [AudioPluginFactoryEntryPoint]
    public static AudioPluginFactory GetFactory()
    {
        var factory = new AudioPluginFactory(new("My Company", "https://plugin_corp.com", "contact@plugin_corp.com"));
        factory.RegisterPlugin<HelloWorldProcessor>(new (ClassId, "HelloWorld", AudioPluginCategory.Effect));
        factory.RegisterPlugin<HelloWorldController>(new(HelloWorldController.ClassId, "HelloWorld Controller", AudioPluginCategory.Effect));
        return factory;
    }

    public override Guid ControllerId => HelloWorldController.ClassId;

    protected override bool Initialize(AudioProcessorSetup setup)
    {
        setup.AddDefaultStereoAudioInput();
        setup.AddDefaultStereoAudioOutput();
        return true;
    }

    protected override void SaveState(PortableBinaryWriter writer)
    {
    }

    protected override void RestoreState(PortableBinaryReader reader)
    {
    }

    protected override void Process(in AudioProcessSetupData setupData, in AudioProcessData data)
    {

    }
}

public class HelloWorldController : AudioController
{
    public static readonly Guid ClassId = new("B4752A4E-C4FC-40BB-9FCE-42178135A69E");


    protected override bool Initialize(AudioControllerSetup setup)
    {
        setup.AddParameter(new(1, "Delay", "Delay", "ms"));
        return true;
    }

    protected override void SaveState(PortableBinaryWriter writer)
    {
        throw new NotImplementedException();
    }

    protected override void RestoreState(PortableBinaryReader reader)
    {
        throw new NotImplementedException();
    }
}