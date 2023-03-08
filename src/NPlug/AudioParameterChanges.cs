// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using NPlug.Backend;

namespace NPlug;

/// <summary>
/// Defines parameter changes.
/// </summary>
public readonly ref struct AudioParameterChanges
{
    private readonly IAudioParameterChangesBackend? _backend;
    internal readonly IntPtr NativeContext;

    internal AudioParameterChanges(IAudioParameterChangesBackend backend, IntPtr nativeContext)
    {
        _backend = backend;
        NativeContext = nativeContext;
    }

    /// <summary>
    /// Gets the number of parameter changes.
    /// </summary>
    public int Count {
        get
        {
            return NativeContext == nint.Zero || _backend is null ? 0 : _backend!.GetParameterCount(this);
        }
    }

    /// <summary>
    /// Gets the parameter value queue at the specified index.
    /// </summary>
    /// <param name="index">The index of the parameter.</param>
    /// <returns>The associated queue.</returns>
    [UnscopedRef]
    public AudioParameterValueQueue GetParameterData(int index)
    {
        return GetSafeBackend().GetParameterData(this, index);
    }

    /// <summary>
    /// Creates a new parameter change change.
    /// </summary>
    /// <param name="parameterId">The parameter id.</param>
    /// <param name="index">The output index of the queue.</param>
    /// <returns>The parameter value queue.</returns>
    [UnscopedRef]
    public AudioParameterValueQueue AddParameterData(AudioParameterId parameterId, out int index)
    {
        return GetSafeBackend().AddParameterData(this, parameterId, out index);
    }

    private IAudioParameterChangesBackend GetSafeBackend()
    {
        if (_backend is null || NativeContext == nint.Zero) ThrowNotInitialized();
        return _backend;
    }

    [DoesNotReturn]
    private static void ThrowNotInitialized()
    {
        throw new InvalidOperationException("This parameter changes is not initialized");
    }
}