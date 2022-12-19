// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.SimpleProgramChange;

/// <summary>
/// Port of C/C++ program change sample from https://github.com/steinbergmedia/vst3_public_sdk/tree/master/samples/vst/programchange/source
/// </summary>
public class SimpleProgramChangeProcessor : AudioProcessor<SimpleProgramChangeModel>
{
    public static readonly Guid ClassId = new("d2d46df8-3397-4acf-9008-df3396b890f2");

    public SimpleProgramChangeProcessor() : base(AudioSampleSizeSupport.Float32)
    {
    }

    public override Guid ControllerClassId => SimpleProgramChangeController.ClassId;


    protected override bool Initialize(AudioHostApplication host)
    {
        AddDefaultStereoAudioInput();
        AddDefaultStereoAudioOutput();
        AddDefaultEventInput();
        return true;
    }

    protected override void ProcessMain(in AudioProcessData data)
    {
        // Parameter changes and ByPass are handled automatically by AudioProcessor

        // Changing the program will make the gain ranging from 0 to 1
        // See SimpleProgramChangeModel
        var gain = (float)Model.Gain.NormalizedValue;

        var inputBus = data.Input[0];
        var outputBus = data.Output[0];

        for (int channel = 0; channel < inputBus.ChannelCount; channel++)
        {
            var sampleFrames = data.SampleCount;
            var input = inputBus.GetChannelSpanAsFloat32(ProcessSetupData, data, channel);
            var output = outputBus.GetChannelSpanAsFloat32(ProcessSetupData, data, channel);
            for(int sample = 0; sample < sampleFrames; sample++)
            {
                // apply gain
                output[sample] = input[sample] * gain;
            }
        }
    }
}