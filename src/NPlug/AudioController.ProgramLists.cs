// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace NPlug;

public abstract partial class AudioController<TAudioRootUnit>
{
    int IAudioControllerUnitInfo.ProgramListCount => RootUnit.ProgramListCount;

    AudioProgramListInfo IAudioControllerUnitInfo.GetProgramListInfo(int listIndex)
    {
        return RootUnit.GetProgramListByIndex(listIndex).Info;
    }

    string IAudioControllerUnitInfo.GetProgramName(AudioProgramListId listId, int programIndex)
    {
        return RootUnit.GetProgramListById(listId).Name;
    }

    bool IAudioControllerUnitInfo.TryGetProgramInfo(AudioProgramListId listId, int programIndex, string attributeId, [NotNullWhen(true)] out string? attributeValue)
    {
        return RootUnit.GetProgramListById(listId).Attributes.TryGetValue(attributeId, out attributeValue);
    }

    bool IAudioControllerUnitInfo.HasProgramPitchNames(AudioProgramListId listId, int programIndex)
    {
        return RootUnit.GetProgramListById(listId)[programIndex].PitchNames.Count > 0;
    }

    bool IAudioControllerUnitInfo.TryGetProgramPitchName(AudioProgramListId listId, int programIndex, short midiPitch, [NotNullWhen(true)] out string? pitchName)
    {
        return RootUnit.GetProgramListById(listId)[programIndex].PitchNames.TryGetValue(midiPitch, out pitchName);
    }
}