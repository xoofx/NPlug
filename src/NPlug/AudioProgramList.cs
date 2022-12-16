// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace NPlug;

public class AudioProgramList : IReadOnlyList<AudioProgram>
{
    private readonly List<AudioProgram> _programs;

    internal AudioProgramList(string name, int tag = 0)
    {
        Name = name;
        _programs = new List<AudioProgram>();
        Id = tag;
        Attributes = new Dictionary<string, string>();
    }

    public string Name { get; set; }

    public int Count  => _programs.Count;

    public AudioProgramListId Id { get; internal set; }

    public AudioProgramListInfo Info => new (Id, Name, Count);

    public Dictionary<string, string> Attributes { get; }
    
    public AudioProgram this[int index] => _programs[index];

    public void Add(AudioProgram program)
    {
        _programs.Add(program);
    }

    public List<AudioProgram>.Enumerator GetEnumerator()
    {
        return _programs.GetEnumerator();
    }

    IEnumerator<AudioProgram> IEnumerable<AudioProgram>.GetEnumerator()
    {
        return _programs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_programs).GetEnumerator();
    }
}