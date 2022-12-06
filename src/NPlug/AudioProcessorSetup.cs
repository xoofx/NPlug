// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

public readonly ref struct AudioProcessorSetup
{
    private readonly AudioProcessor _processor;

    internal AudioProcessorSetup(AudioProcessor processor, AudioHostApplication host)
    {
        _processor = processor;
        Host = host;
    }

    public AudioHostApplication Host { get; }

    public void AddDefaultStereoAudioInput()
    {
        AddAudioInput("Stereo Input", SpeakerArrangement.SpeakerStereo);
    }

    public void AddDefaultStereoAudioOutput()
    {
        AddAudioOutput("Stereo Output", SpeakerArrangement.SpeakerStereo);
    }
    
    public void AddDefaultEventInput()
    {
        AddEventInput("Event Input", 1);
    }

    public void AddAudioInput(string name, SpeakerArrangement speaker, BusType busType = BusType.Main, BusFlags flags = BusFlags.DefaultActive)
    {
        AssertInitialize();
        _processor.AudioInputBuses.Add(new AudioBusInfo(name, speaker, BusDirection.Input, busType, flags));
    }

    public void AddAudioOutput(string name, SpeakerArrangement speaker, BusType busType = BusType.Main, BusFlags flags = BusFlags.DefaultActive)
    {
        AssertInitialize();
        _processor.AudioOutputBuses.Add(new AudioBusInfo(name, speaker, BusDirection.Output, busType, flags));
    }

    public void AddEventInput(string name, int channelCount, BusType busType = BusType.Main, BusFlags flags = BusFlags.DefaultActive)
    {
        AssertInitialize();
        _processor.EventInputBuses.Add(new EventBusInfo(name, channelCount, BusDirection.Input, busType, flags));
    }

    public void AddEventOutput(string name, int channelCount, BusType busType = BusType.Main, BusFlags flags = BusFlags.DefaultActive)
    {
        AssertInitialize();
        _processor.EventOutputBuses.Add(new EventBusInfo(name, channelCount, BusDirection.Output, busType, flags));
    }

    private void AssertInitialize()
    {
        if (_processor is null) throw new InvalidOperationException($"Invalid {nameof(AudioProcessorSetup)}. Must be used only from {nameof(AudioProcessor)}.Initialize");
    }
}