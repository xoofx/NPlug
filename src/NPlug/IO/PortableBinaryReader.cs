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

/// <summary>
/// A portable binary reader that can be used to read data from a <see cref="Stream"/>. This class is not thread-safe.
/// </summary>
[SkipLocalsInit]
public class PortableBinaryReader : IDisposable
{
    internal PortableBinaryReader() : this(Stream.Null, false)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="PortableBinaryReader"/> with the specified <see cref="Stream"/>.
    /// </summary>
    public PortableBinaryReader(Stream stream, bool owned = true)
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
    /// Reads an enum of the specified type from the stream.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <returns>The value of the enum.</returns>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <summary>
    /// Reads a byte from the stream.
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <summary>
    /// Reads a boolean from the stream.
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <summary>
    /// Reads a 16-bit unsigned integer from the stream.
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <summary>
    /// Reads a 16-bit signed integer from the stream.
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <summary>
    /// Reads a 32-bit unsigned integer from the stream.
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <summary>
    /// Reads a 32-bit signed integer from the stream.
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <summary>
    /// Reads a 64-bit unsigned integer from the stream.
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <summary>
    /// Reads a 64-bit signed integer from the stream.
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <summary>
    /// Reads a 32-bit floating point number from the stream.
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <summary>
    /// Reads a 64-bit floating point number from the stream.
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <summary>
    /// Reads a string from the stream.
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
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

    /// <inheritdoc />
    public void Dispose()
    {
        if (Owned)
        {
            Stream.Dispose();
        }
    }
}