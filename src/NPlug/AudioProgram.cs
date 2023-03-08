// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using NPlug.IO;

namespace NPlug;

/// <summary>
/// Defines a program for an <see cref="AudioProgramList"/>.
/// </summary>
public class AudioProgram
{
    private Stream? _stream;
    private long _originalPosition;

    /// <summary>
    /// Creates a new instance of this program.
    /// </summary>
    public AudioProgram(string name)
    {
        Name = name;
        PitchNames = new Dictionary<short, string>();
        Attributes = new Dictionary<string, string>();
    }

    /// <summary>
    /// Gets the name of this program.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the index of this program (once registered in a <see cref="AudioProgramList"/>).
    /// </summary>
    public int Index { get; internal set; }

    /// <summary>
    /// Gets the associated <see cref="AudioProgramList"/> or null if not registered.
    /// </summary>
    public AudioProgramList? Parent { get; internal set; }

    /// <summary>
    /// Gets the pitch names associated with this program.
    /// </summary>
    public Dictionary<short, string> PitchNames { get; }

    /// <summary>
    /// Gets the attributes associated with this program.
    /// </summary>
    public Dictionary<string, string> Attributes { get; }

    /// <summary>
    /// Sets the program data from the specified unit.
    /// </summary>
    /// <param name="model">The unit providing the data.</param>
    public void SetProgramDataFromUnit(AudioUnit model)
    {
        var writer = new PortableBinaryWriter(new MemoryStream(), false);
        model.Save(writer, AudioProcessorModelStorageMode.SkipProgramChangeParameters);
        _stream = writer.Stream;
        _stream.Position = 0;
        _originalPosition = 0;
    }

    /// <summary>
    /// Gets the program data associated with this program.
    /// </summary>
    public Stream? GetProgramData()
    {
        if (_stream is { })
        {
            _stream.Position = _originalPosition;
        }
        return _stream;
    }

    /// <summary>
    /// Sets the program data from the specified stream.
    /// </summary>
    /// <param name="stream">The data to copy from.</param>
    public void SetProgramDataFromStream(Stream stream)
    {
        var memoryStream = new MemoryStream();
        if (stream.CanSeek)
        {
            memoryStream.Capacity = (int)stream.Length;
        }
        stream.CopyTo(memoryStream);
        _stream = memoryStream;
        memoryStream.Position = 0;
        _originalPosition = 0;
    }
}