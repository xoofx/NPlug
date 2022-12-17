// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// MIDI controllers are not transmitted directly to a VST component.MIDI as hardware protocol has
/// restrictions that can be avoided in software.Controller data in particular come along with unclear
/// and often ignored semantics.On top of this they can interfere with regular parameter automation and
/// the host is unaware of what happens in the plug-in when passing MIDI controllers directly.
/// So any functionality that is to be controlled by MIDI controllers must be exported as regular parameter.
/// The host will transform incoming MIDI controller data using this interface and transmit them as regular
/// parameter change.This allows the host to automate them in the same way as other parameters.
/// <see cref="AudioMidiControllerNumber"/> can be a typical MIDI controller value extended to some others values like pitchbend or
/// aftertouch.
/// If the mapping has changed, the plug-in must call <see cref="IAudioControllerHandler.RestartComponent"/>
/// to inform the host about this change.
/// </summary>
/// <remarks>
/// VST Method from `IMidiMapping`.
/// </remarks>
public interface IAudioControllerMidiMapping : IAudioController
{
    /// <summary>
    /// Gets an (preferred) associated <see cref="AudioParameterId"/> for a given Input Event Bus index, channel and MIDI Controller.
    /// </summary>
    /// <param name="busIndex">Index of Input Event Bus</param>
    /// <param name="channel">Channel of the bus</param>
    /// <param name="midiControllerNumber"><see cref="AudioMidiControllerNumber"/> for expected values (could be bigger than 127)</param>
    /// <param name="id">Return the associated ParamID to the given midiControllerNumber</param>
    bool TryGetMidiControllerAssignment(int busIndex, int channel, AudioMidiControllerNumber midiControllerNumber, out AudioParameterId id);

}