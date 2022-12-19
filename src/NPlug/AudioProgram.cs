// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using NPlug.IO;

namespace NPlug;

public class AudioProgram
{
    private Stream? _stream;
    private long _originalPosition;
    
    public AudioProgram(string name)
    {
        Name = name;
        PitchNames = new Dictionary<short, string>();
        Attributes = new Dictionary<string, string>();
    }

    public string Name { get; }

    public int Index { get; internal set; }

    public AudioProgramList? Parent { get; internal set; }

    public Dictionary<short, string> PitchNames { get; }
    
    public Dictionary<string, string> Attributes { get; }
    
    public void SetProgramDataFromUnit(AudioUnit model)
    {
        var writer = new PortableBinaryWriter(new MemoryStream(), false);
        model.Save(writer, AudioProcessorModelStorageMode.SkipProgramChangeParameters);
        _stream = writer.Stream;
        _stream.Position = 0;
        _originalPosition = 0;
    }

    public Stream? GetProgramData()
    {
        if (_stream is { })
        {
            _stream.Position = _originalPosition;
        }
        return _stream;
    }

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