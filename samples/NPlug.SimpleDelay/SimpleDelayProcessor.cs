// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug.SimpleDelay;

/// <summary>
/// Port of C/C++ adelay sample from https://github.com/steinbergmedia/vst3_public_sdk/tree/master/samples/vst/adelay/source
/// </summary>
public class SimpleDelayProcessor : AudioProcessor<SimpleDelayModel>
{
    private float[] _bufferLeft;
    private float[] _bufferRight;
    private int _bufferPosition;

    public static readonly Guid ClassId = new("7a130e07-004a-408d-a1d8-97b671f36ca1");

    public SimpleDelayProcessor() : base(AudioSampleSizeSupport.Float32)
    {
        _bufferLeft = Array.Empty<float>();
        _bufferRight = Array.Empty<float>();
    }

    public override Guid ControllerClassId => SimpleDelayController.ClassId;


    protected override bool Initialize(AudioHostApplication host)
    {
        AddAudioInput("AudioInput", SpeakerArrangement.SpeakerStereo);
        AddAudioOutput("AudioOutput", SpeakerArrangement.SpeakerStereo);
        return true;
    }

    protected override void OnActivate(bool isActive)
    {
        if (isActive)
        {
            var delayInSamples = (int)(ProcessSetupData.SampleRate * sizeof(float) + 0.5);
            _bufferLeft = GC.AllocateArray<float>(delayInSamples, true);
            _bufferRight = GC.AllocateArray<float>(delayInSamples, true);
            _bufferPosition = 0;
        }
        else
        {
            _bufferLeft = Array.Empty<float>();
            _bufferRight = Array.Empty<float>();
            _bufferPosition = 0;
        }
    }

    protected override void ProcessMain(in AudioProcessData data)
    {
        var delayInSamples = Math.Max(1, (int)(ProcessSetupData.SampleRate * Model.Delay.NormalizedValue));
        for (int channel = 0; channel < 2; channel++)
        {
            var inputChannel = data.Input[0].GetChannelSpanAsFloat32(ProcessSetupData, data, channel);
            var outputChannel = data.Output[0].GetChannelSpanAsFloat32(ProcessSetupData, data, channel);

            var sampleCount = data.SampleCount;
            var buffer = channel == 0 ? _bufferLeft : _bufferRight;
            var tempBufferPos = _bufferPosition;
            for (int sample = 0; sample < sampleCount; sample++)
            {
                var tempSample = inputChannel[sample];
                outputChannel[sample] = buffer[tempBufferPos];
                buffer[tempBufferPos] = tempSample;
                tempBufferPos++;
                if (tempBufferPos >= delayInSamples)
                {
                    tempBufferPos = 0;
                }
            }
        }

        _bufferPosition += data.SampleCount;
        while (_bufferPosition >= delayInSamples)
        {
            _bufferPosition -= delayInSamples;
        }
    }
}
