using System.Runtime.CompilerServices;

namespace NPlug;

public static class HelloWorldPlugin
{
    public static AudioPluginFactory GetFactory()
    {
        var factory = new AudioPluginFactory(new("My Company", "https://plugin_corp.com", "contact@plugin_corp.com"));
        factory.RegisterPlugin<HelloWorldProcessor>(new(HelloWorldProcessor.ClassId, "HelloWorld", AudioProcessorCategory.Effect));
        factory.RegisterPlugin<HelloWorldController>(new(HelloWorldController.ClassId, "HelloWorld Controller"));
        return factory;
    }

    [ModuleInitializer]
    internal static void ExportThisPlugin()
    {
        AudioPluginFactoryExporter.Instance = GetFactory();
    }
    
    public class HelloWorldProcessor : AudioProcessor<HelloWorldModel>
    {
        public static readonly Guid ClassId = new("493cc995-b69e-40b8-bf6f-fc7a57134fd1");

        public HelloWorldProcessor() : base(AudioSampleSizeSupport.Any)
        {
        }


        public override Guid ControllerClassId => HelloWorldController.ClassId;

        protected override bool Initialize(AudioHostApplication host)
        {
            AddDefaultStereoAudioInput();
            AddDefaultStereoAudioOutput();
            AddDefaultEventInput();
            return true;
        }
    }

    public class HelloWorldController : AudioController<HelloWorldModel>
    {
        public static readonly Guid ClassId = new("B4752A4E-C4FC-40BB-9FCE-42178135A69E");

        public HelloWorldController()
        {
        }

        protected override bool Initialize(AudioHostApplication host)
        {
            SetMidiCCMapping(AudioMidiControllerNumber.ModWheel, Model.ModWheelParameter);
            return true;
        }
    }

    public class HelloWorldModel : AudioProcessorModel
    {
        public HelloWorldModel() : base("HelloWorld")
        {
            SubUnit1 = AddUnit(new AudioUnit("SubUnit1"));
            SubUnit11 = SubUnit1.AddUnit(new AudioUnit("SubUnit1.1"));
            SubUnit2 = AddUnit(new AudioUnit("SubUnit2"));
            SubUnit21 = SubUnit2.AddUnit(new AudioUnit("SubUnit2.1"));

            DelayParameter = AddParameter(new AudioParameter("Delay", units: "ms"));
            HelloParameter = AddParameter(new AudioRangeParameter("hello", minValue: 2000.0, maxValue: 20480.0, defaultPlainValue: 4000.0));
            ListParameter = AddParameter(new AudioStringListParameter("List", new[] { "A", "B", "C" }));

            ModWheelParameter = SubUnit1.AddParameter(new AudioParameter("Mod Wheel"));
        }

        public AudioParameter DelayParameter { get; }

        public AudioRangeParameter HelloParameter { get; }

        public AudioStringListParameter ListParameter { get; }

        public AudioParameter ModWheelParameter { get; }

        public AudioUnit SubUnit1 { get; }
        public AudioUnit SubUnit11 { get; }

        public AudioUnit SubUnit2 { get; }
        public AudioUnit SubUnit21 { get; }
    }
}


