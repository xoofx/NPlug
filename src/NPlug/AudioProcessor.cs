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

public abstract class AudioProcessor : AudioPluginComponent, IAudioProcessor
{
    internal readonly List<BusInfo> AudioInputBuses;
    internal readonly List<BusInfo> AudioOutputBuses;
    internal readonly List<BusInfo> EventInputBuses;
    internal readonly List<BusInfo> EventOutputBuses;

    private AudioProcessSetupData _processSetupData;
    private PortableBinaryReader? _streamReader;
    private PortableBinaryWriter? _streamWriter;

    protected AudioProcessor(AudioSampleSizeSupport sampleSizeSupport) : this(sampleSizeSupport, 0, 0)
    {
    }

    protected AudioProcessor(AudioSampleSizeSupport sampleSizeSupport, uint latencySamples, uint tailSamples)
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

    public abstract Guid ControllerId { get; }

    public InputOutputMode InputOutputMode { get; private set; }

    public uint LatencySamples { get; }

    public uint TailSamples { get; }

    public bool IsActive { get; private set; }

    public bool IsProcessing { get; private set; }
    
    protected bool IsSampleSizeSupported(AudioSampleSize sampleSize)
    {
        return (SampleSizeSupport & (AudioSampleSizeSupport)(1 << (int)sampleSize)) != 0;
    }

    protected abstract bool Initialize(AudioProcessorSetup setup);

    protected virtual bool TryGetBusRoutingInfo(in BusRoutingInfo inInfo, out BusRoutingInfo outInfo)
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
    protected ReadOnlySpan<BusInfo> GetBusInfoList(BusMediaType type, BusDirection dir)
    {
        var list = type == BusMediaType.Audio
            ? (dir == BusDirection.Input ? AudioInputBuses : AudioOutputBuses)
            : (dir == BusDirection.Input ? EventInputBuses : EventOutputBuses);

        return CollectionsMarshal.AsSpan(list);
    }

    internal override bool InitializeInternal(AudioHostApplication hostApplication)
    {
        return Initialize(new AudioProcessorSetup(this, hostApplication));
    }

    void IAudioProcessor.SetInputOutputMode(InputOutputMode mode)
    {
        InputOutputMode = mode;
    }

    bool IAudioProcessor.TryGetBusRoutingInfo(in BusRoutingInfo inInfo, out BusRoutingInfo outInfo)
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

    void IAudioProcessor.Process(in AudioProcessData processData)
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