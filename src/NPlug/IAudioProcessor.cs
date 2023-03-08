// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;

namespace NPlug;

/// <summary>
/// Interface of an audio processor.
/// </summary>
public interface IAudioProcessor : IAudioPluginComponent
{
    /// <summary>
    /// Called before initializing the component to get information about the controller class.
    /// </summary>
    /// <value></value>
    Guid ControllerClassId { get; }

    /// <summary>
    /// Gets the requirement flags for the content of the <see cref="AudioProcessContext"/> during processing audio.
    /// </summary>
    /// <remarks>
    /// Implementation of VST3 `IProcessContextRequirements`.
    /// </remarks>
    AudioProcessContextRequirementFlags ProcessContextRequirementFlags { get; }

    /// <summary>
    /// Called before 'initialize' to set the component usage (optional). See \ref IoModes.
    /// </summary>
    /// <param name="mode"></param>
    void SetInputOutputMode(InputOutputMode mode);

    /// <summary>
    /// Called after the plug-in is initialized. See \ref MediaTypes, BusDirections.
    /// </summary>
    int GetBusCount(BusMediaType type, BusDirection dir);

    /// <summary>
    /// Called after the plug-in is initialized. See \ref MediaTypes, BusDirections.
    /// </summary>
    BusInfo GetBusInfo(BusMediaType type, BusDirection dir, int index);

    /// <summary>
    /// Retrieves routing information (to be implemented when more than one regular input or output bus exists).
    /// </summary>
    /// <param name="inInfo">always refers to an input bus while the returned outInfo must refer to an output bus!</param>
    /// <param name="outInfo">The output routing info associated to the input routing.</param>
    /// <returns><c>true</c> if the routing info is available; otherwise <c>false</c>.</returns>
    bool TryGetBusRoutingInfo(in BusRoutingInfo inInfo, out BusRoutingInfo outInfo);

    /// <summary>
    /// Called upon (de-)activating a bus in the host application. The plug-in should only processed
    /// an activated bus, the host could provide less see \ref AudioBusBuffers in the process call
    /// (<see cref="Process"/>) if last busses are not activated.An already activated bus
    /// does not need to be reactivated after a <see cref="SetBusArrangements"/> call.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="dir"></param>
    /// <param name="index"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    void ActivateBus(BusMediaType type, BusDirection dir, int index, bool state);

    /// <summary>
    /// Activates / deactivates the component.
    /// </summary>
    /// <param name="state"></param>
    void SetActive(bool state);

    /// <summary>
    /// Sets complete state of component.
    /// </summary>
    /// <param name="streamInput"></param>
    void SetState(Stream streamInput);

    /// <summary>
    /// Retrieves complete state of component.
    /// </summary>
    /// <param name="streamOutput"></param>
    void GetState(Stream streamOutput);

    /// <summary>
    /// Try to set(host = &gt; plug-in) a wanted arrangement for inputs and outputs.
    /// The host should always deliver the same number of input and output buses than the plug-in
    /// needs (see @ref IComponent::getBusCount). The plug-in has 3 possibilities to react on this
    /// setBusArrangements call:@n 1. The plug-in accepts these arrangements, then it should modify, if needed, its buses to match 
    /// these new arrangements (later on asked by the host with IComponent::getBusInfo () or
    /// IAudioProcessor::getBusArrangement ()) and then should return kResultTrue.@n 2. The plug-in does not accept or support these requested arrangements for all
    /// inputs/outputs or just for some or only one bus, but the plug-in can try to adapt its current
    /// arrangements according to the requested ones (requested arrangements for kMain buses should be
    /// handled with more priority than the ones for kAux buses), then it should modify its buses arrangements
    /// and should return kResultFalse.@n 3. Same than the point 2 above the plug-in does not support these requested arrangements 
    /// but the plug-in cannot find corresponding arrangements, the plug-in could keep its current arrangement
    /// or fall back to a default arrangement by modifying its buses arrangements and should return kResultFalse.@n
    /// </summary>
    bool SetBusArrangements(Span<SpeakerArrangement> inputs, Span<SpeakerArrangement> outputs);

    /// <summary>
    /// Gets the bus arrangement for a given direction (input/output) and index.
    /// </summary>
    SpeakerArrangement GetBusArrangement(BusDirection direction, int index);


    /// <summary>
    /// Asks if a given sample size is supported see @ref SymbolicSampleSizes.
    /// </summary>
    bool CanProcessSampleSize(AudioSampleSize sampleSize);

    /// <summary>
    /// Gets the current Latency in samples.
    /// The returned value defines the group delay or the latency of the plug-in. For example, if the plug-in internally needs
    /// to look in advance (like compressors) 512 samples then this plug-in should report 512 as latency.
    /// If during the use of the plug-in this latency change, the plug-in has to inform the host by
    /// using IComponentHandler::restartComponent (kLatencyChanged), this could lead to audio playback interruption
    /// because the host has to recompute its internal mixer delay compensation.
    /// Note that for player live recording this latency should be zero or small.
    /// </summary>
    uint LatencySamples { get; }

    /// <summary>
    /// Gets tail size in samples. For example, if the plug-in is a Reverb plug-in and it knows that
    /// the maximum length of the Reverb is 2sec, then it has to return in getTailSamples() 
    /// (in VST2 it was getGetTailSize ()): 2*sampleRate.
    /// This information could be used by host for offline processing, process optimization and 
    /// downmix (avoiding signal cut (clicks)).
    /// </summary>
    uint TailSamples { get; }

    /// <summary>
    /// Called in disable state (setActive not called with true) before setProcessing is called and processing will begin.
    /// </summary>
    public bool SetupProcessing(in AudioProcessSetupData processSetupData);

    /// <summary>
    /// Informs the plug-in about the processing state. This will be called before any process calls
    /// start with true and after with false.
    /// Note that setProcessing (false) may be called after setProcessing (true) without any process
    /// calls.
    /// Note this function could be called in the UI or in Processing Thread, thats why the plug-in
    /// should only light operation (no memory allocation or big setup reconfiguration), 
    /// this could be used to reset some buffers (like Delay line or Reverb).
    /// The host has to be sure that it is called only when the plug-in is enable (setActive (true)
    /// was called).
    /// </summary>
    /// <param name="state"></param>
    void SetProcessing(bool state);

    /// <summary>
    /// The Process call, where all information (parameter changes, event, audio buffer) are passed.
    /// </summary>
    /// <param name="processData"></param>
    void Process(in AudioProcessData processData);

    // IAudioPresentationLatency

    /// <summary>
    /// Informs the plug-in about the Audio Presentation Latency in samples for a given direction (kInput/kOutput) and bus index.
    /// </summary>
    /// <remarks>
    /// Implementation of VST3 `IAudioPresentationLatency`.
    /// </remarks>
    void SetAudioPresentationLatencySamples(BusDirection dir, int busIndex, uint latencyInSamples);
}