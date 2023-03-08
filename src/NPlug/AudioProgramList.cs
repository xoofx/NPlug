// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace NPlug;

/// <summary>
/// Defines a list of <see cref="AudioProgram"/>.
/// </summary>
public sealed class AudioProgramList : IReadOnlyList<AudioProgram>
{
    private readonly List<AudioProgram> _programs;

    /// <summary>
    /// Creates an instance of this list.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <param name="programListCapacity"></param>
    public AudioProgramList(string name, int id = 0, int programListCapacity = 0)
    {
        Name = name;
        _programs = new List<AudioProgram>(programListCapacity);
        Id = id;
    }

    /// <summary>
    /// Gets or sets the name of this program list.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Get the number of programs.
    /// </summary>
    public int Count  => _programs.Count;

    /// <summary>
    /// Gets a boolean indicating whether this list has been initialized.
    /// </summary>
    public bool Initialized { get; internal set; }

    /// <summary>
    /// Gets the associated id of this program list.
    /// </summary>
    public AudioProgramListId Id { get; internal set; }

    /// <summary>
    /// Gets the associated info of this program list.
    /// </summary>
    public AudioProgramListInfo Info => new (Id, Name, Count);

    /// <summary>
    /// Gets the program at the specified index.
    /// </summary>
    /// <param name="index">Index of the program.</param>
    /// <returns>The associated program.</returns>
    public AudioProgram this[int index] => _programs[index];

    /// <summary>
    /// Adds a program.
    /// </summary>
    /// <param name="program"></param>
    /// <exception cref="ArgumentException"></exception>
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

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
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

    private void AssertInitialized()
    {
        if (Initialized) throw new InvalidOperationException($"Cannot modify this program list {Id} with name {Name} as it is already initialized");
    }
}