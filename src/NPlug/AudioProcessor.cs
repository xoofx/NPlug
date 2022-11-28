// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NPlug.IO;

namespace NPlug;

public abstract class AudioProcessor : AudioPlugin, IAudioProcessor
{
    internal readonly List<BusInfo> AudioInputBuses;
    internal readonly List<BusInfo> AudioOutputBuses;
    internal readonly List<BusInfo> EventInputBuses;
    internal readonly List<BusInfo> EventOutputBuses;

    private AudioProcessSetupData _processSetupData;
    private PortableBinaryReader? _streamReader;
    private PortableBinaryWriter? _streamWriter;

    protected AudioProcessor(AudioSampleSizeSupport sampleSizeSupport, uint latencySamples = 0, uint tailSamples = 0)
    {
        SampleSizeSupport = sampleSizeSupport;
        AudioInputBuses = new List<BusInfo>();
        AudioOutputBuses = new List<BusInfo>();
        EventInputBuses = new List<BusInfo>();
        EventOutputBuses = new List<BusInfo>();
        LatencySamples = latencySamples;
        TailSamples = tailSamples;
    }
    
    public AudioSampleSizeSupport SampleSizeSupport { get; }

    public virtual Guid ControllerId => Guid.Empty;

    public AudioInputOutputMode InputOutputMode { get; private set; }

    public uint LatencySamples { get; }

    public uint TailSamples { get; }

    public bool IsActive { get; private set; }

    public bool IsProcessing { get; private set; }
    
    protected bool IsSampleSizeSupported(AudioSampleSize sampleSize)
    {
        return (SampleSizeSupport & (AudioSampleSizeSupport)(1 << (int)sampleSize)) != 0;
    }

    protected abstract bool Initialize(AudioProcessorSetup setup);

    protected virtual bool TryGetBusRoutingInfo(in AudioBusRoutingInfo inInfo, out AudioBusRoutingInfo outInfo)
    {
        outInfo = default;
        return false;
    }
    
    protected virtual void OnActivate(bool isActive)
    {
    }

    protected virtual bool OnBusActivate(BusInfo busInfo, bool newActiveState)
    {
        return newActiveState;
    }

    protected virtual void OnSetupProcessing(in AudioProcessSetupData processSetupData)
    {
    }

    protected abstract void SaveState(PortableBinaryWriter writer);

    protected abstract void RestoreState(PortableBinaryReader reader);

    protected abstract void Process(in AudioProcessSetupData setupData, in AudioProcessData data);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected ReadOnlySpan<BusInfo> GetBusInfoList(AudioBusMediaType type, AudioBusDirection dir)
    {
        var list = type == AudioBusMediaType.Audio
            ? (dir == AudioBusDirection.Input ? AudioInputBuses : AudioOutputBuses)
            : (dir == AudioBusDirection.Input ? EventInputBuses : EventOutputBuses);

        return CollectionsMarshal.AsSpan(list);
    }

    internal override bool InitializeInternal(AudioHostApplication hostApplication)
    {
        return Initialize(new AudioProcessorSetup(this, hostApplication));
    }


    void IAudioProcessor.SetInputOutputMode(AudioInputOutputMode mode)
    {
        InputOutputMode = mode;
    }

    bool IAudioProcessor.TryGetBusRoutingInfo(in AudioBusRoutingInfo inInfo, out AudioBusRoutingInfo outInfo)
    {
        return TryGetBusRoutingInfo(inInfo, out outInfo);
    }
    
    void IAudioProcessor.SetState(Stream streamInput)
    {
        var reader = _streamReader;
        if (reader is null)
        {
            reader = new PortableBinaryReader(streamInput);
            _streamReader = reader;
        }
        else
        {
            reader.Stream = streamInput;
        }
        RestoreState(reader);
    }

    void IAudioProcessor.GetState(Stream streamOutput)
    {
        var writer = _streamWriter;
        if (writer is null)
        {
            writer = new PortableBinaryWriter(streamOutput);
            _streamWriter = writer;
        }
        else
        {
            writer.Stream = streamOutput;
        }
        SaveState(writer);
    }

    bool IAudioProcessor.SetBusArrangements(Span<SpeakerArrangement> inputs, Span<SpeakerArrangement> outputs)
    {
        if (inputs.Length > AudioInputBuses.Count ||
            outputs.Length > AudioOutputBuses.Count) return false;

        for (int i = 0; i < inputs.Length; i++)
        {
            ((AudioBusInfo)AudioInputBuses[i]).SpeakerArrangement = inputs[i];
        }

        for (int i = 0; i < outputs.Length; i++)
        {
            ((AudioBusInfo)AudioOutputBuses[i]).SpeakerArrangement = outputs[i];
        }

        return true;
    }

    SpeakerArrangement IAudioProcessor.GetBusArrangement(AudioBusDirection direction, int index)
    {
        var busInfo = (AudioBusInfo)GetBusInfoList(AudioBusMediaType.Audio, direction)[index];
        return busInfo.SpeakerArrangement;
    }

    int IAudioProcessor.GetBusCount(AudioBusMediaType type, AudioBusDirection dir)
    {
        return GetBusInfoList(type, dir).Length;
    }
    
    BusInfo IAudioProcessor.GetBusInfo(AudioBusMediaType type, AudioBusDirection dir, int index)
    {
        return GetBusInfoList(type, dir)[index];
    }

    bool IAudioProcessor.ActivateBus(AudioBusMediaType type, AudioBusDirection dir, int index, bool state)
    {
        var busInfo = GetBusInfoList(type, dir)[index];
        busInfo.IsActive = OnBusActivate(busInfo, state);
        return busInfo.IsActive;
    }

    bool IAudioProcessor.CanProcessSampleSize(AudioSampleSize sampleSize) => IsSampleSizeSupported(sampleSize);

    void IAudioProcessor.SetActive(bool state)
    {
        IsActive = state;
        OnActivate(state);
    }

    bool IAudioProcessor.SetupProcessing(in AudioProcessSetupData processSetupData)
    {
        if (IsSampleSizeSupported(processSetupData.SampleSize))
        {
            _processSetupData = processSetupData;
            OnSetupProcessing(processSetupData);
            return true;
        }

        return false;
    }

    void IAudioProcessor.SetProcessing(bool state)
    {
        IsProcessing = state;
    }

    void IAudioProcessor.Process(ref AudioProcessData processData)
    {
        Process(in _processSetupData, in processData);
    }

    internal override void TerminateInternal()
    {
        AudioInputBuses.Clear();
        AudioOutputBuses.Clear();
        EventInputBuses.Clear();
        EventOutputBuses.Clear();
        base.TerminateInternal();
    }
}