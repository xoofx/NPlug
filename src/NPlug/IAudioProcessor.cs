// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;

namespace NPlug;

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

    /** Called after the plug-in is initialized. See \ref MediaTypes, BusDirections */
    int GetBusCount(BusMediaType type, BusDirection dir);

    /** Called after the plug-in is initialized. See \ref MediaTypes, BusDirections */
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

    bool SetBusArrangements(Span<SpeakerArrangement> inputs, Span<SpeakerArrangement> outputs);

    SpeakerArrangement GetBusArrangement(BusDirection direction, int index);

    bool CanProcessSampleSize(AudioSampleSize sampleSize);

    uint LatencySamples { get; }

    uint TailSamples { get; }

    public bool SetupProcessing(in AudioProcessSetupData processSetupData);

    void SetProcessing(bool state);

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