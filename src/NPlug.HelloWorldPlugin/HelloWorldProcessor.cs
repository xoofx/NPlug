namespace NPlug;

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