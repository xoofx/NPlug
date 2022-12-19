// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NPlug.IO;

public class PortableBinaryWriter : IDisposable
{
    internal PortableBinaryWriter() : this(Stream.Null, false)
    {
    }

    public PortableBinaryWriter(Stream stream, bool owned = true)
    {
        Stream = stream;
        Owned = owned;
    }

    public Stream Stream { get; set; }

    public bool Owned { get; set; }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteEnum<T>(T data) where T : unmanaged, Enum
    {
        var span = new Span<byte>(&data, sizeof(T));
        if (!BitConverter.IsLittleEndian)
        {
            span.Reverse();
        }
        Stream.Write(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteByte(byte data)
    {
        Stream.Write(new Span<byte>(&data, 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteBool(bool data)
    {
        Stream.Write(new Span<byte>(&data, 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteUInt16(ushort data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        } 
        Stream.Write(new Span<byte>(&data, 2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteInt16(short data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        Stream.Write(new Span<byte>(&data, 2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteUInt32(uint data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        Stream.Write(new Span<byte>(&data, 4));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteInt32(int data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        Stream.Write(new Span<byte>(&data, 4));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteUInt64(ulong data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        Stream.Write(new Span<byte>(&data, 8));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteInt64(long data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        Stream.Write(new Span<byte>(&data, 8));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteFloat32(float data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(BitConverter.SingleToInt32Bits(data)));
        }
        Stream.Write(new Span<byte>(&data, 4));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteFloat64(double data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(BitConverter.DoubleToInt64Bits(data)));
        }
        Stream.Write(new Span<byte>(&data, 8));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(string data)
    {
        WriteInt32(data.Length);
        if (data.Length > 0)
        {
            var span = MemoryMarshal.Cast<char, byte>(data.AsSpan());
            Stream.Write(span);
        }
    }

    public void Dispose()
    {
        if (Owned)
        {
            Stream.Dispose();
        }
    }
}