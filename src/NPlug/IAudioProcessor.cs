// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;

namespace NPlug;


// Discussions https://forums.steinberg.net/t/clarification-of-parameter-handling-in-vst-3/201914
// https://steinbergmedia.github.io/vst3_dev_portal/pages/Technical+Documentation/API+Documentation/Index.html
// [3.0.0] Interfaces supported by the plug-in
// https://steinbergmedia.github.io/vst3_dev_portal/pages/Technical+Documentation/Change+History/3.0.0/Plug+in+Interfaces.html
// [3.0.0] Interfaces supported by the host
// https://steinbergmedia.github.io/vst3_dev_portal/pages/Technical+Documentation/Change+History/3.0.0/Host+Interfaces.html

// TODO: To add
// - IProcessContextRequirements
// - IAudioPresentationLatency
// 


public interface IAudioProcessor : IAudioPluginComponent
{
    /// <summary>
    /// Called before initializing the component to get information about the controller class.
    /// </summary>
    /// <value></value>
    Guid ControllerId { get; }

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
    bool ActivateBus(BusMediaType type, BusDirection dir, int index, bool state);

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
}