// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NPlug;

public abstract partial class AudioProcessor<TAudioProcessorModel>
{
    protected virtual bool SetBusArrangements(Span<SpeakerArrangement> inputs, Span<SpeakerArrangement> outputs)
    {
        if (AudioInputBuses.Count != inputs.Length || AudioOutputBuses.Count != outputs.Length) return false;

        for (int i = 0; i < inputs.Length; i++)
        {
            if (((AudioBusInfo)AudioInputBuses[i]).SpeakerArrangement != inputs[i])
            {
                return false;
            }
        }

        for (int i = 0; i < outputs.Length; i++)
        {
            if (((AudioBusInfo)AudioOutputBuses[i]).SpeakerArrangement != outputs[i])
            {
                return false;
            }
        }

        return true;
    }

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
        AudioInputBuses.Add(new AudioBusInfo(name, speaker, BusDirection.Input, busType, flags));
    }

    public void AddAudioOutput(string name, SpeakerArrangement speaker, BusType busType = BusType.Main, BusFlags flags = BusFlags.DefaultActive)
    {
        AudioOutputBuses.Add(new AudioBusInfo(name, speaker, BusDirection.Output, busType, flags));
    }

    public void AddEventInput(string name, int channelCount, BusType busType = BusType.Main, BusFlags flags = BusFlags.DefaultActive)
    {
        EventInputBuses.Add(new EventBusInfo(name, channelCount, BusDirection.Input, busType, flags));
    }

    public void AddEventOutput(string name, int channelCount, BusType busType = BusType.Main, BusFlags flags = BusFlags.DefaultActive)
    {
        EventOutputBuses.Add(new EventBusInfo(name, channelCount, BusDirection.Output, busType, flags));
    }
    
    protected virtual void OnAudioBusPresentationLatencyChanged(AudioBusInfo busInfo, uint previousPresentationLatencyInSamples)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadOnlySpan<BusInfo> GetBusInfoList(BusMediaType type, BusDirection dir)
    {
        var list = type == BusMediaType.Audio
            ? (dir == BusDirection.Input ? AudioInputBuses : AudioOutputBuses)
            : (dir == BusDirection.Input ? EventInputBuses : EventOutputBuses);

        return CollectionsMarshal.AsSpan(list);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadOnlySpan<BusInfo> GetAudioOutputBuses() => GetBusInfoList(BusMediaType.Audio, BusDirection.Output);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadOnlySpan<BusInfo> GetAudioInputBuses() => GetBusInfoList(BusMediaType.Audio, BusDirection.Input);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadOnlySpan<BusInfo> GetEventOutputBuses() => GetBusInfoList(BusMediaType.Event, BusDirection.Output);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadOnlySpan<BusInfo> GetEventInputBuses() => GetBusInfoList(BusMediaType.Event, BusDirection.Input);
    
    bool IAudioProcessor.SetBusArrangements(Span<SpeakerArrangement> inputs, Span<SpeakerArrangement> outputs)
    {
        return SetBusArrangements(inputs, outputs);
    }

    SpeakerArrangement IAudioProcessor.GetBusArrangement(BusDirection direction, int index)
    {
        var busInfo = (AudioBusInfo)GetBusInfoList(BusMediaType.Audio, direction)[index];
        return busInfo.SpeakerArrangement;
    }

    int IAudioProcessor.GetBusCount(BusMediaType type, BusDirection dir)
    {
        return GetBusInfoList(type, dir).Length;
    }

    BusInfo IAudioProcessor.GetBusInfo(BusMediaType type, BusDirection dir, int index)
    {
        return GetBusInfoList(type, dir)[index];
    }

    void IAudioProcessor.ActivateBus(BusMediaType type, BusDirection dir, int index, bool state)
    {
        var busInfo = GetBusInfoList(type, dir)[index];
        busInfo.IsActive = OnBusActivate(busInfo, state);
    }
}