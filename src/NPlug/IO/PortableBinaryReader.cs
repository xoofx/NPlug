// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NPlug.IO;

[SkipLocalsInit]
public class PortableBinaryReader : IDisposable
{
    internal PortableBinaryReader() : this(Stream.Null, false)
    {
    }

    public PortableBinaryReader(Stream stream, bool owned = true)
    {
        Owned = owned;
        Stream = Stream.Null;
    }

    public bool Owned { get; set; }

    public Stream Stream { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe T ReadEnum<T>() where T : unmanaged, Enum
    {
        T data;
        var span = new Span<byte>(&data, sizeof(T));
        if (Stream.Read(span) != sizeof(T))
        {
            throw new EndOfStreamException();
        }

        if (!BitConverter.IsLittleEndian)
        {
            span.Reverse();
        }
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe byte ReadByte()
    {
        byte data;
        if (Stream.Read(new Span<byte>(&data, 1)) != 1)
        {
            throw new EndOfStreamException();
        }

        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe bool ReadBool()
    {
        bool data;
        if (Stream.Read(new Span<byte>(&data, 1)) != 1)
        {
            throw new EndOfStreamException();
        }

        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ushort ReadUInt16()
    {
        ushort data;
        if (Stream.Read(new Span<byte>(&data, 2)) != 2)
        {
            throw new EndOfStreamException();
        }
        return BitConverter.IsLittleEndian ? data : BinaryPrimitives.ReverseEndianness(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe short ReadInt16()
    {
        short data;
        if (Stream.Read(new Span<byte>(&data, 2)) != 2)
        {
            throw new EndOfStreamException();
        }
        return BitConverter.IsLittleEndian ? data : BinaryPrimitives.ReverseEndianness(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe uint ReadUInt32()
    {
        uint data;
        if (Stream.Read(new Span<byte>(&data, 4)) != 4)
        {
            throw new EndOfStreamException();
        }
        return BitConverter.IsLittleEndian ? data : BinaryPrimitives.ReverseEndianness(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int ReadInt32()
    {
        int data;
        if (Stream.Read(new Span<byte>(&data, 4)) != 4)
        {
            throw new EndOfStreamException();
        }
        return BitConverter.IsLittleEndian ? data : BinaryPrimitives.ReverseEndianness(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ulong ReadUInt64()
    {
        ulong data;
        if (Stream.Read(new Span<byte>(&data, 8)) != 8)
        {
            throw new EndOfStreamException();
        }
        return BitConverter.IsLittleEndian ? data : BinaryPrimitives.ReverseEndianness(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe long ReadInt64()
    {
        long data;
        if (Stream.Read(new Span<byte>(&data, 8)) != 8)
        {
            throw new EndOfStreamException();
        }
        return BitConverter.IsLittleEndian ? data : BinaryPrimitives.ReverseEndianness(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe float ReadFloat32()
    {
        int data;
        if (Stream.Read(new Span<byte>(&data, 4)) != 4)
        {
            throw new EndOfStreamException();
        }
        return BitConverter.IsLittleEndian ? BitConverter.Int32BitsToSingle(data) : BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(data));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe double ReadFloat64()
    {
        long data;
        if (Stream.Read(new Span<byte>(&data, 8)) != 8)
        {
            throw new EndOfStreamException();
        }
        return BitConverter.IsLittleEndian ? BitConverter.Int64BitsToDouble(data) : BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(data));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString()
    {
        int length = ReadInt32();
        if (length == 0) return string.Empty;
        // TODO: use stackalloc
        var buffer = ArrayPool<byte>.Shared.Rent(length * 2);
        try
        {
            if (Stream.Read(buffer) != length * 2)
            {
                throw new EndOfStreamException();
            }
            return new string(MemoryMarshal.Cast<byte, char>(buffer.AsSpan(0, length * 2)));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
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