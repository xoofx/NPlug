// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using NPlug.Backend;

namespace NPlug;

public readonly ref struct AudioParameterChanges
{
    private readonly IAudioParameterChangesBackend? _backend;
    internal readonly IntPtr NativeContext;

    internal AudioParameterChanges(IAudioParameterChangesBackend backend, IntPtr nativeContext)
    {
        _backend = backend;
        NativeContext = nativeContext;
    }

    public int Count {
        get
        {
            return NativeContext == IntPtr.Zero ? 0 : _backend!.GetParameterCount(this);
        }
    }

    [UnscopedRef]
    public AudioParameterValueQueue GetParameterData(int index)
    {
        return GetSafeBackend().GetParameterData(this, index);
    }

    [UnscopedRef]
    public AudioParameterValueQueue AddParameterData(AudioParameterId parameterId, out int index)
    {
        return GetSafeBackend().AddParameterData(this, parameterId, out index);
    }

    private IAudioParameterChangesBackend GetSafeBackend()
    {
        if (_backend is null) ThrowNotInitialized();
        return _backend;
    }

    [DoesNotReturn]
    private static void ThrowNotInitialized()
    {
        throw new InvalidOperationException("This parameter changes is not initialized");
    }
}

public readonly record struct AudioParameterId(int Value)
{
    public static implicit operator AudioParameterId(int value) => new(value);
}

public readonly record struct AudioUnitId(int Value)
{
    public static readonly AudioUnitId NoParent = new (-1);
    public static implicit operator AudioUnitId(int value) => new(value);
}
