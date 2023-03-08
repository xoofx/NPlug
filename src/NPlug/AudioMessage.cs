// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using NPlug.Backend;

namespace NPlug;

/// <summary>
/// An audio message to send to the host.
/// </summary>
public readonly ref struct AudioMessage
{
    private readonly IAudioMessageBackend? _backend;
    internal readonly IntPtr NativeContext;
    internal readonly AudioAttributeList AttributeList;

    internal AudioMessage(IAudioMessageBackend backend, IntPtr nativeContext, AudioAttributeList attributeList)
    {
        _backend = backend;
        NativeContext = nativeContext;
        AttributeList = attributeList;
    }

    /// <summary>
    /// Gets or sets the id of this message.
    /// </summary>
    public string Id
    {
        get => GetSafeBackend().GetId(this);
        set => GetSafeBackend().SetId(this, value);
    }

    /// <summary>
    /// Gets the attributes associated with this message.
    /// </summary>
    public AudioAttributeList Attributes => AttributeList;

    /// <summary>
    /// Dispose this message.
    /// </summary>
    public void Dispose()
    {
        if (_backend is null) return;
        GetSafeBackend().Destroy(this);
    }

    private IAudioMessageBackend GetSafeBackend()
    {
        if (_backend is null) ThrowNotInitialized();
        return _backend;
    }

    [DoesNotReturn]
    private static void ThrowNotInitialized()
    {
        throw new InvalidOperationException("This message is not initialized");
    }
}