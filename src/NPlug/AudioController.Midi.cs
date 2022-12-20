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
    public Dictionary<AudioMidiControllerNumber, AudioParameter> MapMidiCCToAudioParameter { get; }

    public void SetMidiCCMapping(AudioMidiControllerNumber midiControllerNumber, AudioParameter parameter)
    {
        MapMidiCCToAudioParameter[midiControllerNumber] = parameter;
    }

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