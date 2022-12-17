// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System;
using NPlug.Backend;

namespace NPlug;

public readonly ref struct AudioEventList
{
    private readonly IAudioEventListBackend? _backend;
    internal readonly IntPtr NativeContext;

    internal AudioEventList(IAudioEventListBackend? backend, IntPtr nativeContext)
    {
        NativeContext = nativeContext;
        _backend = backend;
    }

    /// <summary>
    /// Returns the count of events.
    /// </summary>
    public int Count => NativeContext != nint.Zero && _backend != null ? _backend.GetEventCount(this) : 0;

    /// <summary>
    /// Gets parameter by index.
    /// </summary>
    public bool TryGetEvent(int index, out AudioEvent evt)
    {
        return GetSafeBackend().TryGetEvent(this, index, out evt);
    }

    /// <summary>
    /// Adds a new event.
    /// </summary>
    public bool TryAddEvent(IntPtr context, in AudioEvent e)
    {
        return GetSafeBackend().TryAddEvent(this, e);
    }

    private IAudioEventListBackend GetSafeBackend()
    {
        if (NativeContext == nint.Zero || _backend is null) ThrowNotInitialized();
        return _backend;
    }

    [DoesNotReturn]
    private static void ThrowNotInitialized()
    {
        throw new InvalidOperationException("This event list is empty.");
    }
}