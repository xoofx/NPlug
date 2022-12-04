// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Buffers;
using System.Text;

namespace NPlug.Vst3;

internal static partial class LibVst
{
    public readonly struct TempUtf8String : IDisposable
    {
        public readonly byte[] Buffer;

        public TempUtf8String(string text)
        {
            Buffer = ArrayPool<byte>.Shared.Rent(text.Length * 3 + 1);
            var length = Encoding.UTF8.GetBytes(text, Buffer);
            Buffer[length] = (byte)'\0';
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Buffer);
        }
    }
}