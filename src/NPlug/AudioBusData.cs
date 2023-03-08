// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// Provides audio buffers for audio buses. This is used by <see cref="AudioProcessData.Input"/> and <see cref="AudioProcessData.Output"/> during <see cref="IAudioProcessor.Process"/> is called.
/// </summary>
public readonly unsafe ref struct AudioBusData
{
    /// <summary>
    /// Creates a new instance of this struct.
    /// </summary>
    public AudioBusData(int busCount, AudioBusBuffers* audioBuffers, AudioParameterChanges parameterChanges, AudioEventList events)
    {
        BusCount = busCount;
        _audioBuffers = audioBuffers;
        ParameterChanges = parameterChanges;
        Events = events;
    }

    /// <summary>
    /// The number of bus.
    /// </summary>
    public readonly int BusCount;

    // Internal pointer, use the indexer to retrieve safely the buffer.
    private readonly AudioBusBuffers* _audioBuffers;

    /// <summary>
    /// Gets the parameter changes.
    /// </summary>
    public readonly AudioParameterChanges ParameterChanges;

    /// <summary>
    /// Gets the event list.
    /// </summary>
    public readonly AudioEventList Events;

    /// <summary>
    /// Gets the buffer at the specified bus index.
    /// </summary>
    /// <param name="busIndex">The index of the bus.</param>
    /// <returns>The audio buffer.</returns>
    public ref AudioBusBuffers this[int busIndex] => ref GetBufferByBusIndex(busIndex);

    /// <summary>
    /// Gets the buffer at the specified bus index.
    /// </summary>
    /// <param name="busIndex">The index of the bus.</param>
    /// <returns>The audio buffer.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the bus index is outside of the <see cref="BusCount"/>.</exception>
    public ref AudioBusBuffers GetBufferByBusIndex(int busIndex)
    {
        if ((uint)busIndex >= (uint)BusCount) throw new ArgumentOutOfRangeException(nameof(busIndex));
        return ref _audioBuffers[busIndex];
    }
}