// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NPlug.IO;

public class PortableBinaryWriter
{
    private Stream _stream;

    public PortableBinaryWriter(Stream stream)
    {
        _stream = stream;
    }

    public Stream Stream
    {
        get => _stream;
        set => _stream = value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteEnum<T>(T data) where T : unmanaged, Enum
    {
        var span = new Span<byte>(&data, sizeof(T));
        if (!BitConverter.IsLittleEndian)
        {
            span.Reverse();
        }
        _stream.Write(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteByte(byte data)
    {
        _stream.Write(new Span<byte>(&data, 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteBool(bool data)
    {
        _stream.Write(new Span<byte>(&data, 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteUInt16(ushort data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        } 
        _stream.Write(new Span<byte>(&data, 2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteInt16(short data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        _stream.Write(new Span<byte>(&data, 2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteUInt32(uint data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        _stream.Write(new Span<byte>(&data, 4));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteInt32(int data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        _stream.Write(new Span<byte>(&data, 4));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteUInt64(ulong data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        _stream.Write(new Span<byte>(&data, 8));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteInt64(long data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        _stream.Write(new Span<byte>(&data, 8));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteFloat32(float data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(BitConverter.SingleToInt32Bits(data)));
        }
        _stream.Write(new Span<byte>(&data, 4));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteFloat64(double data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(BitConverter.DoubleToInt64Bits(data)));
        }
        _stream.Write(new Span<byte>(&data, 8));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(string data)
    {
        WriteInt32(data.Length);
        if (data.Length > 0)
        {
            var span = MemoryMarshal.Cast<char, byte>(data.AsSpan());
            _stream.Write(span);
        }
    }
}