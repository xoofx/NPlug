// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace NPlug;

public abstract partial class AudioController<TAudioControllerModel>
{
    int IAudioControllerUnitInfo.ProgramListCount => Model.ProgramListCount;

    AudioProgramListInfo IAudioControllerUnitInfo.GetProgramListInfo(int listIndex)
    {
        return Model.GetProgramListByIndex(listIndex).Info;
    }

    string IAudioControllerUnitInfo.GetProgramName(AudioProgramListId listId, int programIndex)
    {
        return Model.GetProgramListById(listId).Name;
    }

    bool IAudioControllerUnitInfo.TryGetProgramInfo(AudioProgramListId listId, int programIndex, string attributeId, [NotNullWhen(true)] out string? attributeValue)
    {
        return Model.GetProgramListById(listId).Attributes.TryGetValue(attributeId, out attributeValue);
    }

    bool IAudioControllerUnitInfo.HasProgramPitchNames(AudioProgramListId listId, int programIndex)
    {
        return Model.GetProgramListById(listId)[programIndex].PitchNames.Count > 0;
    }

    bool IAudioControllerUnitInfo.TryGetProgramPitchName(AudioProgramListId listId, int programIndex, short midiPitch, [NotNullWhen(true)] out string? pitchName)
    {
        return Model.GetProgramListById(listId)[programIndex].PitchNames.TryGetValue(midiPitch, out pitchName);
    }
}