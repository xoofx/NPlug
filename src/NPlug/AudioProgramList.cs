// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace NPlug;

public class AudioProgramList : IReadOnlyList<AudioProgram>
{
    private readonly List<AudioProgram> _programs;
    public AudioProgramList(string name, int id = 0, int programListCapacity = 0)
    {
        Name = name;
        _programs = new List<AudioProgram>(programListCapacity);
        Id = id;
    }

    public string Name { get; set; }

    public int Count  => _programs.Count;

    public bool Initialized { get; internal set; }

    public AudioProgramListId Id { get; internal set; }

    public AudioProgramListInfo Info => new (Id, Name, Count);
    
    public AudioProgram this[int index] => _programs[index];

    public void Add(AudioProgram program)
    {
        AssertInitialized();
        if (program.Parent != null)
        {
            throw new ArgumentException("The program is already attached to a list");
        }
        program.Index = _programs.Count;
        _programs.Add(program);
        program.Parent = this;
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

    protected void AssertInitialized()
    {
        if (Initialized) throw new InvalidOperationException($"Cannot modify this program list {Id} with name {Name} as it is already initialized");
    }
}