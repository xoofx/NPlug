// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NPlug;

public abstract partial class AudioController<TAudioRootUnit>
{
    private readonly List<AudioProgramList> _programLists;
    private readonly Dictionary<AudioProgramListId, int> _mapProgramIdToIndex;

    public AudioProgramList GetProgramList(int index) => _programLists[index];

    public AudioProgramList GetProgramListById(AudioProgramListId id) => _programLists[_mapProgramIdToIndex[id]];

    public bool ContainsProgramList(AudioProgramListId id) => _mapProgramIdToIndex.ContainsKey(id);

    public AudioProgramList AddProgramList(string name, int tag = 0)
    {
        AudioProgramListId requestedId = tag == 0 ? _programLists.Count + 1 : tag;
        if (_mapProgramIdToIndex.ContainsKey(requestedId))
        {
            throw new ArgumentException($"A program list with the same id {requestedId} is already added to this controller");
        }
        _mapProgramIdToIndex[requestedId] = _programLists.Count;
        var programList = new AudioProgramList(name, requestedId.Value);
        _programLists.Add(programList);
        return programList;
    }

    int IAudioControllerUnitInfo.ProgramListCount => _programLists.Count;

    AudioProgramListInfo IAudioControllerUnitInfo.GetProgramListInfo(int listIndex)
    {
        return GetProgramList(listIndex).Info;
    }

    string IAudioControllerUnitInfo.GetProgramName(AudioProgramListId listId, int programIndex)
    {
        return GetProgramListById(listId)[programIndex].Name;
    }

    bool IAudioControllerUnitInfo.TryGetProgramInfo(AudioProgramListId listId, int programIndex, string attributeId, [NotNullWhen(true)] out string? attributeValue)
    {
        return GetProgramListById(listId).Attributes.TryGetValue(attributeId, out attributeValue);
    }

    bool IAudioControllerUnitInfo.HasProgramPitchNames(AudioProgramListId listId, int programIndex)
    {
        return GetProgramListById(listId)[programIndex].PitchNames.Count > 0;
    }

    bool IAudioControllerUnitInfo.TryGetProgramPitchName(AudioProgramListId listId, int programIndex, short midiPitch, [NotNullWhen(true)] out string? pitchName)
    {
        return GetProgramListById(listId)[programIndex].PitchNames.TryGetValue(midiPitch, out pitchName);
    }
}