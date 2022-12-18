// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// Flags used for <see cref="IAudioControllerHandler.RestartComponent"/>
/// </summary>
[Flags]
public enum AudioRestartFlags
{
    /// <summary>
    /// The Component should be reloaded
    /// The host has to unload completely the plug-in (controller/processor) and reload it. 
    /// [SDK 3.0.0]
    /// </summary>
    ReloadComponent = 1 << 0,

    /// <summary>
    /// Input / Output Bus configuration has changed
    /// The plug-in informs the host that either the bus configuration or the bus count has changed.
    /// The host has to deactivate the plug-in, asks the plug-in for its wanted new bus configurations,
    /// adapts its processing graph and reactivate the plug-in.
    /// [SDK 3.0.0]
    /// </summary>
    IoChanged = 1 << 1,

    /// <summary>
    /// Multiple parameter values have changed  (as result of a program change for example)
    /// The host invalidates all caches of parameter values and asks the edit controller for the current values.
    /// [SDK 3.0.0]
    /// </summary>
    ParamValuesChanged = 1 << 2,

    /// <summary>
    /// Latency has changed
    /// The plug informs the host that its latency has changed, getLatencySamples should return the new latency after setActive (true) was called
    /// The host has to deactivate and reactivate the plug-in, then afterwards the host could ask for the current latency (getLatencySamples)
    /// see IAudioProcessor::getLatencySamples
    /// [SDK 3.0.0]
    /// </summary>
    LatencyChanged = 1 << 3,

    /// <summary>
    /// Parameter titles, default values or flags (ParameterFlags) have changed
    /// The host invalidates all caches of parameter infos and asks the edit controller for the current infos.
    /// [SDK 3.0.0]
    /// </summary>
    ParamTitlesChanged = 1 << 4,

    /// <summary>
    /// MIDI Controllers and/or Program Changes Assignments have changed
    /// The plug-in informs the host that its MIDI-CC mapping has changed (for example after a MIDI learn or new loaded preset) 
    /// or if the stepCount or UnitID of a ProgramChange parameter has changed.
    /// The host has to rebuild the MIDI-CC =&gt; parameter mapping (getMidiControllerAssignment)
    /// and reread program changes parameters (stepCount and associated unitID)
    /// [SDK 3.0.1]
    /// </summary>
    MidiCCAssignmentChanged = 1 << 5,

    /// <summary>
    /// Note Expression has changed (info, count, PhysicalUIMapping, ...)
    /// Either the note expression type info, the count of note expressions or the physical UI mapping has changed.
    /// The host invalidates all caches of note expression infos and asks the edit controller for the current ones.
    /// See INoteExpressionController, NoteExpressionTypeInfo and INoteExpressionPhysicalUIMapping
    /// [SDK 3.5.0]
    /// </summary>
    NoteExpressionChanged = 1 << 6,

    /// <summary>
    /// Input / Output bus titles have changed
    /// The host invalidates all caches of bus titles and asks the edit controller for the current titles.
    /// [SDK 3.5.0]
    /// </summary>
    IoTitlesChanged = 1 << 7,

    /// <summary>
    /// Prefetch support has changed
    /// The plug-in informs the host that its PrefetchSupport has changed
    /// The host has to deactivate the plug-in, calls IPrefetchableSupport::getPrefetchableSupport and reactivate the plug-in
    /// see IPrefetchableSupport
    /// [SDK 3.6.1]
    /// </summary>
    PrefetchableSupportChanged = 1 << 8,

    /// <summary>
    /// RoutingInfo has changed
    /// The plug-in informs the host that its internal routing (relation of an event-input-channel to an audio-output-bus) has changed
    /// The host ask the plug-in for the new routing with IComponent::getRoutingInfo, @ref vst3Routing see IComponent
    /// [SDK 3.6.6]
    /// </summary>
    RoutingInfoChanged = 1 << 9,

    /// <summary>
    /// Key switches has changed (info, count)
    /// Either the Key switches info, the count of Key switches has changed.
    /// The host invalidates all caches of Key switches infos and asks the edit controller (IKeyswitchController) for the current ones.
    /// See IKeyswitchController
    /// [SDK 3.7.3]
    /// </summary>
    KeySwitchChanged = 1 << 10,
}