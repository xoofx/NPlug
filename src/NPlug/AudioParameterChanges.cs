// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace NPlug;

public readonly ref struct AudioParameterChanges
{
    private readonly IntPtr _nativeContext;
    private readonly IAudioParameterChangesBackend? _backend;
    internal AudioParameterChanges(IntPtr nativeContext, IAudioParameterChangesBackend? backend)
    {
        _nativeContext = nativeContext;
        _backend = backend;
    }

    public int Count {
        get
        {
            return _nativeContext == IntPtr.Zero ? 0 : _backend!.GetParameterCount(_nativeContext);
        }
    }

    public AudioParameterValueQueue GetParameterData(int index)
    {
        if (_nativeContext == IntPtr.Zero) ThrowNotInitialized();
        return _backend!.GetParameterData(_nativeContext, index);
    }

    public AudioParameterValueQueue AddParameterData(AudioParameterId parameterId, out int index)
    {
        if (_nativeContext == IntPtr.Zero) ThrowNotInitialized();
        return _backend!.AddParameterData(_nativeContext, parameterId, out index);
    }

    [DoesNotReturn]
    private static void ThrowNotInitialized()
    {
        throw new InvalidOperationException("This parameter changes is not initialized");
    }
}


public interface IAudioParameterChangesBackend
{
    int GetParameterCount(IntPtr context);
    AudioParameterValueQueue GetParameterData(IntPtr context, int index);
    AudioParameterValueQueue AddParameterData(IntPtr context, AudioParameterId parameterId, out int index);
}

public record struct AudioParameterId(uint Value);

public record struct AudioParameterValue(double Value);

public readonly ref struct AudioParameterValueQueue
{
    private readonly IntPtr _nativeContext;
    private readonly IAudioParameterValueQueueBackend? _backend;
    internal AudioParameterValueQueue(IntPtr nativeContext, IAudioParameterValueQueueBackend? backend)
    {
        _nativeContext = nativeContext;
        _backend = backend;
    }

    public AudioParameterId ParameterId
    {
        get
        {
            if (_nativeContext == IntPtr.Zero) ThrowNotInitialized();
            return _backend!.GetParameterId(_nativeContext);
        }
    }

    public int PointCount
    {
        get
        {
            if (_nativeContext == IntPtr.Zero) ThrowNotInitialized();
            return _backend!.GetPointCount(_nativeContext);
        }
    }

    public AudioParameterValue GetPoint(int index, out int sampleOffset)
    {
        if (_nativeContext == IntPtr.Zero) ThrowNotInitialized();
        return _backend!.GetPoint(_nativeContext, index, out sampleOffset);
    }

    public int AddPoint(int sampleOffset, AudioParameterValue parameterValue)
    {
        if (_nativeContext == IntPtr.Zero) ThrowNotInitialized();
        return _backend!.AddPoint(_nativeContext, sampleOffset, parameterValue);
    }

    [DoesNotReturn]
    private static void ThrowNotInitialized()
    {
        throw new InvalidOperationException("This queue is not initialized");
    }
}


public interface IAudioParameterValueQueueBackend
{
    AudioParameterId GetParameterId(IntPtr context);
    int GetPointCount(IntPtr context);
    AudioParameterValue GetPoint(IntPtr context, int index, out int sampleOffset);
    int AddPoint(IntPtr context, int sampleOffset, AudioParameterValue parameterValue);
}
