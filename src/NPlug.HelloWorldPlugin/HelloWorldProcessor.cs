using System.ComponentModel;
using System.Runtime.InteropServices;
using NPlug.IO;

namespace NPlug.HelloWorldPlugin;



//[AudioPlugin("HelloWorld", AudioPluginCategory.Effect, "493cc995-b69e-40b8-bf6f-fc7a57134fd1")]
public class HelloWorldProcessor : AudioProcessor<HelloWorldUnit>
{
    public static readonly Guid ClassId = new Guid("493cc995-b69e-40b8-bf6f-fc7a57134fd1");
    public HelloWorldProcessor() : base(AudioSampleSizeSupport.Any)
    {
    }
    
    [AudioPluginFactoryEntryPoint]
    public static AudioPluginFactory GetFactory()
    {
        var factory = new AudioPluginFactory(new("My Company", "https://plugin_corp.com", "contact@plugin_corp.com"));
        factory.RegisterPlugin<HelloWorldProcessor>(new (ClassId, "HelloWorld", AudioProcessorCategory.Effect));
        factory.RegisterPlugin<HelloWorldController>(new(HelloWorldController.ClassId, "HelloWorld Controller"));
        return factory;
    }

    public override Guid ControllerId => HelloWorldController.ClassId;

    protected override bool Initialize(AudioHostApplication host)
    {
        AddDefaultStereoAudioInput();
        AddDefaultStereoAudioOutput();
        AddDefaultEventInput();
        return true;
    }

    protected override void Process(in AudioProcessSetupData setupData, in AudioProcessData data)
    {
        var parameterChanges = data.Input.ParameterChanges;
        var count = parameterChanges.Count;
        for (int i = 0; i < count; i++)
        {
            var parameterData = parameterChanges.GetParameterData(i);
            if (parameterData.PointCount > 0 && RootUnit.TryGetParameterById(parameterData.ParameterId, out var parameter))
            {
                // Update with latest parameter
                var value = parameterData.GetPoint(parameterData.PointCount - 1, out _);
                parameter.NormalizedValue = value;
            }
            
        }

        if (data.SampleCount > 0 && RootUnit.ByPassParameter.Value)
        {
            var inputBuffer = data.Input[0];
            var outputBuffer = data.Output[0];

            for (int i = 0; i < inputBuffer.ChannelCount; i++)
            {
                inputBuffer.GetChannelSpanAsBytes(setupData, data, i).CopyTo(outputBuffer.GetChannelSpanAsBytes(setupData, data, i));
            }
        }
    }
}

public class HelloWorldController : AudioController<HelloWorldUnit>
{
    public static readonly Guid ClassId = new("B4752A4E-C4FC-40BB-9FCE-42178135A69E");

    public HelloWorldController()
    {
    }
}

public class HelloWorldUnit : AudioRootUnit
{
    public HelloWorldUnit() : base("HelloWorld")
    {
        SubUnit1 = AddUnit(new AudioUnit("SubUnit1"));
        SubUnit11 = SubUnit1.AddUnit(new AudioUnit("SubUnit1.1"));
        SubUnit2 = AddUnit(new AudioUnit("SubUnit2"));
        SubUnit21 = SubUnit2.AddUnit(new AudioUnit("SubUnit2.1"));

        DelayParameter = AddParameter(new AudioParameter("Delay", units: "ms"));
        HelloParameter = AddParameter(new AudioRangeParameter("hello", minPlainValue: 2000.0, maxPlainValue: 20480.0, defaultPlainValue: 4000.0));
    }

    public AudioParameter DelayParameter { get; }

    public AudioRangeParameter HelloParameter { get; }

    public AudioUnit SubUnit1 { get; }
    public AudioUnit SubUnit11 { get; }

    public AudioUnit SubUnit2 { get; }
    public AudioUnit SubUnit21 { get; }
}