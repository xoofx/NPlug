// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System;

namespace NPlug;

public readonly ref struct AudioEventList
{
    private readonly IntPtr _nativeContext;
    private readonly IAudioEventListBackend? _backend;
    internal AudioEventList(IntPtr nativeContext, IAudioEventListBackend? backend)
    {
        _nativeContext = nativeContext;
        _backend = backend;
    }

    /// <summary>
    /// Returns the count of events.
    /// </summary>
    public int Count
    {
        get
        {
            return _nativeContext == IntPtr.Zero ? 0 : _backend!.GetEventCount(_nativeContext);
        }
    }
    
    /// <summary>
    /// Gets parameter by index.
    /// </summary>
    public bool TryGetEvent(int index, out AudioEvent evt)
    {
        if (_nativeContext == IntPtr.Zero) ThrowNotInitialized();
        return _backend!.TryGetEvent(_nativeContext, index, out evt);
    }

    /// <summary>
    /// Adds a new event.
    /// </summary>
    public bool TryAddEvent(IntPtr context, in AudioEvent e)
    {
        if (_nativeContext == IntPtr.Zero) ThrowNotInitialized();
        return _backend!.TryAddEvent(_nativeContext, e);
    }

    [DoesNotReturn]
    private static void ThrowNotInitialized()
    {
        throw new InvalidOperationException("This event list is empty.");
    }
}

public interface IAudioEventListBackend
{
    /// <summary>
    /// Returns the count of events.
    /// </summary>
    int GetEventCount(IntPtr context);

    /// <summary>
    /// Gets parameter by index.
    /// </summary>
    bool TryGetEvent(IntPtr context, int index, out AudioEvent evt);

    /// <summary>
    /// Adds a new event.
    /// </summary>
    bool TryAddEvent(IntPtr context, in AudioEvent evt);
}