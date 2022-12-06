// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using NPlug.Backend;

namespace NPlug;

public readonly ref struct AudioAttributeList
{
    private readonly IAudioAttributeListBackend? _backend;
    internal readonly IntPtr NativeContext;
    public AudioAttributeList(IAudioAttributeListBackend? backend, IntPtr nativeContext)
    {
        _backend = backend;
        NativeContext = nativeContext;
    }

    public bool TrySetBool(string attributeId, bool value)
    {
        return TrySetInt64(attributeId, value ? 1 : 0);
    }

    public bool TryGetBool(string attributeId, out bool value)
    {
        var result = TryGetInt64(attributeId, out var intValue);
        value = intValue != 0;
        return result;
    }

    public bool TrySetByte(string attributeId, byte value)
    {
        return TrySetInt64(attributeId, value);
    }

    public bool TryGetByte(string attributeId, out byte value)
    {
        var result = TryGetInt64(attributeId, out var intValue);
        value = (byte)intValue;
        return result;
    }

    public bool TrySetInt16(string attributeId, short value)
    {
        return TrySetInt64(attributeId, value);
    }

    public bool TryGetInt16(string attributeId, out short value)
    {
        var result = TryGetInt64(attributeId, out var intValue);
        value = (short)intValue;
        return result;
    }

    public bool TrySetUInt16(string attributeId, ushort value)
    {
        return TrySetInt64(attributeId, value);
    }

    public bool TryGetUInt16(string attributeId, out ushort value)
    {
        var result = TryGetInt64(attributeId, out var intValue);
        value = (ushort)intValue;
        return result;
    }

    public bool TrySetUInt32(string attributeId, uint value)
    {
        return TrySetInt64(attributeId, value);
    }

    public bool TryGetUInt32(string attributeId, out uint value)
    {
        var result = TryGetInt64(attributeId, out var intValue);
        value = unchecked((uint)intValue);
        return result;
    }

    public bool TrySetInt32(string attributeId, int value)
    {
        return TrySetInt64(attributeId, value);
    }

    public bool TryGetInt32(string attributeId, out int value)
    {
        var result = TryGetInt64(attributeId, out var intValue);
        value = unchecked((int)intValue);
        return result;
    }

    public bool TrySetFloat32(string attributeId, float value)
    {
        return TrySetFloat64(attributeId, value);
    }

    public bool TryGetFloat32(string attributeId, out float value)
    {
        var result = TryGetFloat64(attributeId, out var doubleValue);
        value = (float)doubleValue;
        return result;
    }

    public bool TrySetInt64(string attributeId, long value)
    {
        return GetSafeBackend().TrySetInt64(this, attributeId, value);
    }

    public bool TryGetInt64(string attributeId, out long value)
    {
        return GetSafeBackend().TryGetInt64(this, attributeId, out value);
    }

    public bool TrySetFloat64(string attributeId, double value)
    {
        return GetSafeBackend().TrySetFloat64(this, attributeId, value);
    }

    public bool TryGetFloat64(string attributeId, out double value)
    {
        return GetSafeBackend().TryGetFloat64(this, attributeId, out value);
    }

    public bool TrySetString(string attributeId, string value)
    {
        return GetSafeBackend().TrySetString(this, attributeId, value);
    }

    public bool TryGetString(string attributeId, out string value)
    {
        return GetSafeBackend().TryGetString(this, attributeId, out value);
    }

    public bool TrySetBinary(string attributeId, ReadOnlySpan<byte> value)
    {
        return GetSafeBackend().TrySetBinary(this, attributeId, value);
    }

    [UnscopedRef]
    public bool TryGetBinary(string attributeId, out ReadOnlySpan<byte> value)
    {
        return GetSafeBackend().TryGetBinary(this, attributeId, out value);
    }
    
    private IAudioAttributeListBackend GetSafeBackend()
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