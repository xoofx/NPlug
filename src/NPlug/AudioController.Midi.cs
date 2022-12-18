// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;

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
        if (MapMidiCCToAudioParameter.TryGetValue(midiControllerNumber, out var parameter))
        {
            id = parameter.Id;
            return true;
        }

        id = default;
        return false;
    }

    // IMidiMapping
    bool IAudioControllerMidiMapping.TryGetMidiControllerAssignment(int busIndex, int channel, AudioMidiControllerNumber midiControllerNumber, out AudioParameterId id)
    {
        return TryGetMidiControllerAssignment(busIndex, channel, midiControllerNumber, out id);
    }
}