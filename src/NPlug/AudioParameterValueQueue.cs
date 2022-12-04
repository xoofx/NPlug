// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using NPlug.Backend;

namespace NPlug;

public readonly ref struct AudioParameterValueQueue
{
    private readonly IAudioParameterValueQueueBackend? _backend;
    internal readonly IntPtr NativeContext;

    internal AudioParameterValueQueue(IAudioParameterValueQueueBackend backend, IntPtr nativeContext)
    {
        _backend = backend;
        NativeContext = nativeContext;
    }

    public AudioParameterId ParameterId => _backend?.GetParameterId(this) ?? default;

    public int PointCount => _backend?.GetPointCount(this) ?? 0;

    public double GetPoint(int index, out int sampleOffset)
    {
        return GetSafeBackend().GetPoint(this, index, out sampleOffset);
    }

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