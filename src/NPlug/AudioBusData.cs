// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

public readonly unsafe ref struct AudioBusData
{
    public AudioBusData(int bufferCount, AudioBusBuffers* audioBuffers, AudioParameterChanges parameterChanges, AudioEventList events)
    {
        BufferCount = bufferCount;
        _audioBuffers = audioBuffers;
        ParameterChanges = parameterChanges;
        Events = events;
    }

    public readonly int BufferCount;

    private readonly AudioBusBuffers* _audioBuffers;

    public ref AudioBusBuffers this[int index] => ref GetBuffer(index);

    public ref AudioBusBuffers GetBuffer(int index)
    {
        if ((uint)index >= (uint)BufferCount) throw new ArgumentOutOfRangeException(nameof(index));
        return ref _audioBuffers[index];
    }

    public readonly AudioParameterChanges ParameterChanges;

    public readonly AudioEventList Events;
}