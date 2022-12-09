// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    private static void CopyStringToUTF8(string text, byte* dest, int maxLength)
    {
        var encodedLength = Encoding.UTF8.GetBytes(text, new Span<byte>(dest, maxLength));
        // Null terminate
        dest[Math.Min(encodedLength, maxLength - 1)] = 0;
    }

    private static void CopyStringToUTF16(string text, char* dest, int maxLength)
    {
        var lengthToCopy = Math.Min(text.Length, maxLength);
        text.AsSpan(0, lengthToCopy).CopyTo(new Span<char>(dest, lengthToCopy));
        dest[Math.Min(text.Length, maxLength - 1)] = (char)0;
    }
}