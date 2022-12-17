// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace NPlug;

public class AudioProgram
{
    private Stream? _stream;
    private long _originalPosition;
    
    public AudioProgram(string name)
    {
        Name = name;
        PitchNames = new Dictionary<short, string>();
    }

    public string Name { get; }

    public int Index { get; internal set; }

    public AudioProgramList? Parent { get; internal set; }

    public Dictionary<short, string> PitchNames { get; }

    public Stream GetOrLoadProgramData()
    {
        AssertInitialized();
        if (_stream is null)
        {
            _stream = Parent!.LoadProgramDataInternal(Index);
            _originalPosition = _stream.Position;
        }
        _stream.Position = _originalPosition;
        return _stream;
    }

    public void LoadProgramDataFromStream(Stream stream)
    {
        AssertInitialized();
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        _stream = memoryStream;
        _originalPosition = 0;
        Parent!.OnProgramDataChanged(this);
    }

    private void AssertInitialized()
    {
        if (Parent is null) throw new InvalidOperationException($"Cannot load program data {Name} as the program is not attached to a program list");
    }
}