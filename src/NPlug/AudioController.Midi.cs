// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using NPlug.Interop;

namespace NPlug;

public abstract partial class AudioController<TAudioControllerModel>
    : IAudioControllerMidiMapping
{
    /// <summary>
    /// Gets the Mapping of MIDI CC to audio parameters.
    /// </summary>
    public Dictionary<AudioMidiControllerNumber, AudioParameter> MapMidiCCToAudioParameter { get; }

    /// <summary>
    /// Associates a MIDI CC number to an audio parameter.
    /// </summary>
    /// <param name="midiControllerNumber">A MIDI CC number.</param>
    /// <param name="parameter">An audio parameter.</param>
    public void SetMidiCCMapping(AudioMidiControllerNumber midiControllerNumber, AudioParameter parameter)
    {
        MapMidiCCToAudioParameter[midiControllerNumber] = parameter;
    }

    /// <summary>
    /// Gets an (preferred) associated <see cref="AudioParameterId"/> for a given Input Event Bus index, channel and MIDI Controller.
    /// </summary>
    /// <param name="busIndex">The index of the bus.</param>
    /// <param name="channel">The index of the channel.</param>
    /// <param name="midiControllerNumber">The MIDI CC number.</param>
    /// <param name="id">The output parameter id.</param>
    /// <returns><c>true</c> if the <paramref name="midiControllerNumber"/> is associated with a parameter; <c>false</c> otherwise.</returns>
    protected virtual bool TryGetMidiControllerAssignment(int busIndex, int channel, AudioMidiControllerNumber midiControllerNumber, out AudioParameterId id)
    {
        var result = false;
        if (MapMidiCCToAudioParameter.TryGetValue(midiControllerNumber, out var parameter))
        {
            id = parameter.Id;
            result = true;
        }
        else
        {
            id = default;
        }

        // Log more detailed information
        if (InteropHelper.IsTracerEnabled)
        {
            InteropHelper.Tracer?.LogInfo($"{nameof(busIndex)}: {busIndex}, {nameof(channel)}: {channel}, midiCC = {midiControllerNumber.ToText()}{(result ? $" => Parameter Id: {parameter!.Id} Name: {parameter!.Title})": "")}");
        }

        return result;
    }

    // IMidiMapping
    bool IAudioControllerMidiMapping.TryGetMidiControllerAssignment(int busIndex, int channel, AudioMidiControllerNumber midiControllerNumber, out AudioParameterId id)
    {
        return TryGetMidiControllerAssignment(busIndex, channel, midiControllerNumber, out id);
    }
}