// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NPlug.IO;

/// <summary>
/// A portable binary writer that can be used to write data to a stream.
/// </summary>
public class PortableBinaryWriter : IDisposable
{
    internal PortableBinaryWriter() : this(Stream.Null, false)
    {
    }

    /// <summary>
    /// Creates a new instance of this writer.
    /// </summary>
    public PortableBinaryWriter(Stream stream, bool owned = true)
    {
        Stream = stream;
        Owned = owned;
    }

    /// <summary>
    /// Gets or sets associated stream.
    /// </summary>
    public Stream Stream { get; set; }

    /// <summary>
    /// Gets or sets if the <see cref="Stream"/> is owned by this instance and will be disposed when disposing this instance.
    /// </summary>
    public bool Owned { get; set; }

    /// <summary>
    /// Writes an enum to the stream.
    /// </summary>
    /// <typeparam name="T">The type of the enum to write.</typeparam>
    /// <param name="data">The enum data to write.</param>
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

    /// <summary>
    /// Writes the specified byte to the stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteByte(byte data)
    {
        Stream.Write(new Span<byte>(&data, 1));
    }

    /// <summary>
    /// Writes the specified bool to the stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteBool(bool data)
    {
        Stream.Write(new Span<byte>(&data, 1));
    }

    /// <summary>
    /// Write the specified 16-bit unsigned integer to the stream.
    /// </summary>
    /// <param name="data"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteUInt16(ushort data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        } 
        Stream.Write(new Span<byte>(&data, 2));
    }

    /// <summary>
    /// Writes the specified 16-bit signed integer to the stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteInt16(short data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        Stream.Write(new Span<byte>(&data, 2));
    }

    /// <summary>
    /// Writes the specified 32-bit unsigned integer to the stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteUInt32(uint data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        Stream.Write(new Span<byte>(&data, 4));
    }

    /// <summary>
    /// Writes the specified 32-bit signed integer to the stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteInt32(int data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        Stream.Write(new Span<byte>(&data, 4));
    }

    /// <summary>
    /// Writes the specified 64-bit unsigned integer to the stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteUInt64(ulong data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        Stream.Write(new Span<byte>(&data, 8));
    }

    /// <summary>
    /// Writes the specified 64-bit signed integer to the stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteInt64(long data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BinaryPrimitives.ReverseEndianness(data);
        }
        Stream.Write(new Span<byte>(&data, 8));
    }

    /// <summary>
    /// Writes the specified 32-bit floating point number to the stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteFloat32(float data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(BitConverter.SingleToInt32Bits(data)));
        }
        Stream.Write(new Span<byte>(&data, 4));
    }

    /// <summary>
    /// Writes the specified 64-bit floating point number to the stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteFloat64(double data)
    {
        if (!BitConverter.IsLittleEndian)
        {
            data = BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(BitConverter.DoubleToInt64Bits(data)));
        }
        Stream.Write(new Span<byte>(&data, 8));
    }

    /// <summary>
    /// Writes the specified string to the stream.
    /// </summary>
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

    /// <inheritdoc />
    public void Dispose()
    {
        if (Owned)
        {
            Stream.Dispose();
        }
    }
}