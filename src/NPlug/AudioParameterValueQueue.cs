// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using NPlug.Backend;

namespace NPlug;

/// <summary>
/// A queue of parameter values.
/// </summary>
public readonly ref struct AudioParameterValueQueue
{
    private readonly IAudioParameterValueQueueBackend? _backend;
    internal readonly IntPtr NativeContext;

    internal AudioParameterValueQueue(IAudioParameterValueQueueBackend backend, IntPtr nativeContext)
    {
        _backend = backend;
        NativeContext = nativeContext;
    }

    /// <summary>
    /// Gets the associated parameter id for this queue.
    /// </summary>
    public AudioParameterId ParameterId => _backend?.GetParameterId(this) ?? default;

    /// <summary>
    /// Gets the number of point values.
    /// </summary>
    public int PointCount => _backend?.GetPointCount(this) ?? 0;

    /// <summary>
    /// Get a point value at a given index.
    /// </summary>
    /// <param name="index">The index of the point.</param>
    /// <param name="sampleOffset">The output sample offset.</param>
    /// <returns>The point value at the given index.</returns>
    public double GetPoint(int index, out int sampleOffset)
    {
        return GetSafeBackend().GetPoint(this, index, out sampleOffset);
    }

    /// <summary>
    /// Adds a point value at a given sample offset.
    /// </summary>
    /// <param name="sampleOffset">The sample offset this point value applies.</param>
    /// <param name="parameterValue">The point value.</param>
    /// <returns>The index of this point value.</returns>
    public int AddPoint(int sampleOffset, double parameterValue)
    {
        return GetSafeBackend().AddPoint(this, sampleOffset, parameterValue);
    }

    private IAudioParameterValueQueueBackend GetSafeBackend()
    {
        if (_backend is null) ThrowNotInitialized();
        return _backend;
    }

    [DoesNotReturn]
    private static void ThrowNotInitialized()
    {
        throw new InvalidOperationException("This queue is not initialized");
    }
}