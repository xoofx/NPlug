// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NPlug;

public abstract partial class AudioProcessor<TAudioProcessorModel>
{
    /// <summary>
    /// Tries to set the bus arrangements. This method is called by the host. By default, it will check if the number of input and output buses match the number of input and output speaker arrangements.
    /// </summary>
    /// <param name="inputs">The speaker input arrangement.</param>
    /// <param name="outputs">The speaker output arrangement.</param>
    /// <returns><c>true</c> if the arrangement is supported; <c>false</c> otherwise.</returns>
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

    /// <summary>
    /// Adds a default stereo audio input bus.
    /// </summary>
    public void AddDefaultStereoAudioInput()
    {
        AddAudioInput("Stereo Input", SpeakerArrangement.SpeakerStereo);
    }

    /// <summary>
    /// Adds a default stereo audio output bus.
    /// </summary>
    public void AddDefaultStereoAudioOutput()
    {
        AddAudioOutput("Stereo Output", SpeakerArrangement.SpeakerStereo);
    }

    /// <summary>
    /// Adds a default event input bus.
    /// </summary>
    public void AddDefaultEventInput()
    {
        AddEventInput("Event Input", 1);
    }

    /// <summary>
    /// Adds an audio input bus.
    /// </summary>
    /// <param name="name">Name of the bus.</param>
    /// <param name="speaker">The speaker arrangement.</param>
    /// <param name="busType">The kind of bus.</param>
    /// <param name="flags">The flags of the bus.</param>
    public void AddAudioInput(string name, SpeakerArrangement speaker, BusType busType = BusType.Main, BusFlags flags = BusFlags.DefaultActive)
    {
        AudioInputBuses.Add(new AudioBusInfo(name, speaker, BusDirection.Input, busType, flags));
    }

    /// <summary>
    /// Adds an audio output bus.
    /// </summary>
    /// <param name="name">Name of the bus.</param>
    /// <param name="speaker">The speaker arrangement.</param>
    /// <param name="busType">The kind of bus.</param>
    /// <param name="flags">The flags of the bus.</param>
    public void AddAudioOutput(string name, SpeakerArrangement speaker, BusType busType = BusType.Main, BusFlags flags = BusFlags.DefaultActive)
    {
        AudioOutputBuses.Add(new AudioBusInfo(name, speaker, BusDirection.Output, busType, flags));
    }

    /// <summary>
    /// Adds an event input bus.
    /// </summary>
    /// <param name="name">Name of the bus.</param>
    /// <param name="channelCount">The number of channel for this event bus.</param>
    /// <param name="busType">The kind of bus.</param>
    /// <param name="flags">The flags of the bus.</param>
    public void AddEventInput(string name, int channelCount, BusType busType = BusType.Main, BusFlags flags = BusFlags.DefaultActive)
    {
        EventInputBuses.Add(new EventBusInfo(name, channelCount, BusDirection.Input, busType, flags));
    }

    /// <summary>
    /// Adds an event output bus.
    /// </summary>
    /// <param name="name">Name of the bus.</param>
    /// <param name="channelCount">The number of channel for this event bus.</param>
    /// <param name="busType">The kind of bus.</param>
    /// <param name="flags">The flags of the bus.</param>
    public void AddEventOutput(string name, int channelCount, BusType busType = BusType.Main, BusFlags flags = BusFlags.DefaultActive)
    {
        EventOutputBuses.Add(new EventBusInfo(name, channelCount, BusDirection.Output, busType, flags));
    }

    /// <summary>
    /// Called when the latency is changed.
    /// </summary>
    /// <param name="busInfo">The bus affected by this change.</param>
    /// <param name="previousPresentationLatencyInSamples">The previous latency.</param>
    protected virtual void OnAudioBusPresentationLatencyChanged(AudioBusInfo busInfo, uint previousPresentationLatencyInSamples)
    {
    }

    /// <summary>
    /// Gets the BusInfo associated to the specified type and direction.
    /// </summary>
    /// <param name="type">The type of bus (audio or event).</param>
    /// <param name="dir">The direction of the bus (input or output).</param>
    /// <returns>A list of bus info.</returns>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadOnlySpan<BusInfo> GetBusInfoList(BusMediaType type, BusDirection dir)
    {
        var list = type == BusMediaType.Audio
            ? (dir == BusDirection.Input ? AudioInputBuses : AudioOutputBuses)
            : (dir == BusDirection.Input ? EventInputBuses : EventOutputBuses);

        return CollectionsMarshal.AsSpan(list);
    }

    /// <summary>
    /// Gets the list of audio output buses.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadOnlySpan<BusInfo> GetAudioOutputBuses() => GetBusInfoList(BusMediaType.Audio, BusDirection.Output);

    /// <summary>
    /// Gets the list of audio input buses.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadOnlySpan<BusInfo> GetAudioInputBuses() => GetBusInfoList(BusMediaType.Audio, BusDirection.Input);

    /// <summary>
    /// Gets the list of event output buses.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadOnlySpan<BusInfo> GetEventOutputBuses() => GetBusInfoList(BusMediaType.Event, BusDirection.Output);

    /// <summary>
    /// Gets the list of event input buses.
    /// </summary>
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