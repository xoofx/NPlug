// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

public readonly unsafe ref struct AudioBusData
{
    public AudioBusData(int busCount, AudioBusBuffers* audioBuffers, AudioParameterChanges parameterChanges, AudioEventList events)
    {
        BusCount = busCount;
        _audioBuffers = audioBuffers;
        ParameterChanges = parameterChanges;
        Events = events;
    }

    public readonly int BusCount;

    private readonly AudioBusBuffers* _audioBuffers;

    public ref AudioBusBuffers this[int busIndex] => ref GetBufferByBusIndex(busIndex);

    public ref AudioBusBuffers GetBufferByBusIndex(int busIndex)
    {
        if ((uint)busIndex >= (uint)BusCount) throw new ArgumentOutOfRangeException(nameof(busIndex));
        return ref _audioBuffers[busIndex];
    }

    public readonly AudioParameterChanges ParameterChanges;

    public readonly AudioEventList Events;
}