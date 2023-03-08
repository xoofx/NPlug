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

/// <summary>
/// Base class for implementing a <see cref="IAudioProcessor"/>.
/// </summary>
/// <typeparam name="TAudioProcessorModel">The type of the data model associated with this processor.</typeparam>
public abstract partial class AudioProcessor<TAudioProcessorModel>
    : AudioPluginComponent
    , IAudioProcessor
    where TAudioProcessorModel: AudioProcessorModel, new()
{
    internal readonly List<BusInfo> AudioInputBuses;
    internal readonly List<BusInfo> AudioOutputBuses;
    internal readonly List<BusInfo> EventInputBuses;
    internal readonly List<BusInfo> EventOutputBuses;

    private AudioProcessSetupData _processSetupData;
    private PortableBinaryReader? _streamReader;
    private PortableBinaryWriter? _streamWriter;

    /// <summary>
    /// Creates a new instance of this processor.
    /// </summary>
    /// <param name="sampleSizeSupport">Specify the sample size supported by this processor.</param>
    protected AudioProcessor(AudioSampleSizeSupport sampleSizeSupport) : this(sampleSizeSupport, 0, 0, AudioProcessContextRequirementFlags.None)
    {
    }

    /// <summary>
    /// Creates a new instance of this processor.
    /// </summary>
    /// <param name="sampleSizeSupport">Specify the sample size supported by this processor.</param>
    /// <param name="latencySamples">The latency size in samples that this processor would introduce. Default is 0.</param>
    /// <param name="tailSamples">The tail size in samples. See <see cref="TailSamples"/>.</param>
    /// <param name="processContextRequirementFlags">Associated flags.</param>
    protected AudioProcessor(AudioSampleSizeSupport sampleSizeSupport, uint latencySamples, uint tailSamples, AudioProcessContextRequirementFlags processContextRequirementFlags)
    {
        SampleSizeSupport = sampleSizeSupport;
        AudioInputBuses = new List<BusInfo>();
        AudioOutputBuses = new List<BusInfo>();
        EventInputBuses = new List<BusInfo>();
        EventOutputBuses = new List<BusInfo>();
        LatencySamples = latencySamples;
        TailSamples = tailSamples;
        ProcessContextRequirementFlags = processContextRequirementFlags;
        Model = new TAudioProcessorModel();
        Model.Initialize();
    }

    /// <summary>
    /// Gets the sample size support for this processor.
    /// </summary>
    public AudioSampleSizeSupport SampleSizeSupport { get; }

    /// <summary>
    /// Gets an instance of the model associated with this processor.
    /// </summary>
    /// <remarks>
    /// This instance is unique to this processor and is not shared with other processors or controllers.
    /// It must be used from the same thread.
    /// </remarks>
    public TAudioProcessorModel Model { get; }

    /// <summary>
    /// Gets the flags associated with this processor.
    /// </summary>
    public AudioProcessContextRequirementFlags ProcessContextRequirementFlags { get; }

    /// <summary>
    /// Gets the class id of the associated controller <see cref="AudioController{TAudioControllerModel}"/>.
    /// </summary>
    public abstract Guid ControllerClassId { get; }

    /// <summary>
    /// Gets the input/output mode of this processor.
    /// </summary>
    public InputOutputMode InputOutputMode { get; private set; }

    /// <summary>
    /// Gets the latency size in samples.
    /// </summary>
    public uint LatencySamples { get; }

    /// <summary>
    /// Gets the tail size in samples.
    /// For example, if the plug-in is a Reverb plug-in and it knows that the maximum length of the Reverb is 2sec, then it has to return in getTailSamples() (in VST2 it was getGetTailSize ()): 2*sampleRate.
    /// This information could be used by host for offline processing, process optimization and downmix (avoiding signal cut (clicks)). It should return:
    /// - 0 when no tail
    /// - x* sampleRate when x Sec tail.
    /// - <see cref="uint.MaxValue"/> when infinite tail.
    /// </summary>
    public uint TailSamples { get; }

    /// <summary>
    /// Gets a boolean indicating whether this processor is active. This value is set when <see cref="IAudioProcessor.SetActive"/> is called.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the current process setup data. This value is available after <see cref="IAudioProcessor.SetupProcessing"/> is called.
    /// </summary>
    protected ref readonly AudioProcessSetupData ProcessSetupData => ref _processSetupData;

    /// <summary>
    /// Checks if the specified <see cref="AudioSampleSize"/> is supported by this processor.
    /// </summary>
    /// <param name="sampleSize">The sample size.</param>
    /// <returns><c>true</c> if supported.</returns>
    protected bool IsSampleSizeSupported(AudioSampleSize sampleSize)
    {
        return (SampleSizeSupport & (AudioSampleSizeSupport)(1 << (int)sampleSize)) != 0;
    }

    /// <summary>
    /// Called when this processor is initialized.
    /// </summary>
    /// <param name="host">The associated host.</param>
    /// <returns><c>true</c> if the processor is successfully initialized; <c>false</c> otherwise. Default is true.</returns>
    protected virtual bool Initialize(AudioHostApplication host)
    {
        return true;
    }

    /// <summary>
    /// Tries to get bus routing information for the specified bus.
    /// </summary>
    /// <param name="inInfo">The input bus info.</param>
    /// <param name="outInfo">The output routing info.</param>
    /// <returns><c>true</c> if a routing is associated with the specified bus. Default is false.</returns>
    protected virtual bool TryGetBusRoutingInfo(in BusRoutingInfo inInfo, out BusRoutingInfo outInfo)
    {
        outInfo = default;
        return false;
    }

    /// <summary>
    /// This method is called when <see cref="IAudioProcessor.SetActive"/> is called.
    /// </summary>
    /// <param name="isActive">The state of the activation of this processor.</param>
    protected virtual void OnActivate(bool isActive)
    {
    }

    /// <summary>
    /// This method is called when the specified bus is activated.
    /// </summary>
    /// <param name="busInfo">The bus.</param>
    /// <param name="newActiveState">The new state for the bus.</param>
    /// <returns>The state of the bus. Default is <paramref name="newActiveState"/>.</returns>
    protected virtual bool OnBusActivate(BusInfo busInfo, bool newActiveState)
    {
        return newActiveState;
    }

    /// <summary>
    /// This method is called when <see cref="IAudioProcessor.SetupProcessing"/> is called.
    /// </summary>
    protected virtual void OnSetupProcessing(in AudioProcessSetupData processSetupData)
    {
    }

    /// <summary>
    /// This method is called when saving the state of the associated model of this processor.
    /// </summary>
    /// <param name="writer">The output writer.</param>
    protected virtual void SaveState(PortableBinaryWriter writer)
    {
        Model.Save(writer, AudioProcessorModelStorageMode.Default);
    }

    /// <summary>
    /// This method is called when restoring the state to the associated model of this processor.
    /// </summary>
    /// <param name="reader">The output reader.</param>
    protected virtual void RestoreState(PortableBinaryReader reader)
    {
        Model.Load(reader, AudioProcessorModelStorageMode.Default);
    }

    internal override bool InitializeInternal(AudioHostApplication hostApplication)
    {
        return Initialize(hostApplication);
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

    bool IAudioProcessor.CanProcessSampleSize(AudioSampleSize sampleSize) => IsSampleSizeSupported(sampleSize);

    void IAudioProcessor.SetActive(bool state)
    {
        IsActive = state;
        OnActivate(state);
    }

    void IAudioProcessor.SetAudioPresentationLatencySamples(BusDirection dir, int busIndex, uint latencyInSamples)
    {
        var audioBusInfo = (AudioBusInfo)GetBusInfoList(BusMediaType.Audio, dir)[busIndex];
        var previousValue = audioBusInfo.PresentationLatencyInSamples;
        audioBusInfo.PresentationLatencyInSamples = latencyInSamples;
        OnAudioBusPresentationLatencyChanged(audioBusInfo, previousValue);
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