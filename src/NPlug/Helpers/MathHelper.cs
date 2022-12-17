// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace NPlug.Helpers;

internal class MathHelper
{
    /// <summary>
    /// Aligns up the specified value with the specified alignment.
    /// </summary>
    /// <param name="value">The value to align up.</param>
    /// <param name="align">The requested alignment.</param>
    /// <returns>The aligned value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint AlignToUpper(nuint value, uint align)
    {
        var nextValue = ((value + align - 1) / align) * align;
        return nextValue;
    }
}