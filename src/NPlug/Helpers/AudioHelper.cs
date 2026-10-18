// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace NPlug.Helpers;

/// <summary>
/// Helper class.
/// </summary>
public static class AudioHelper
{
    /// <summary>
    /// Checks if the specified buffer is silent.
    /// </summary>
    /// <typeparam name="T">The type of the element (usually float or double).</typeparam>
    /// <param name="buffer">The buffer to check for silence.</param>
    /// <param name="silenceThreshold">The silence threshold.</param>
    /// <returns><c>true</c> if the buffer contains only value below the <paramref name="silenceThreshold"/>.</returns>
    public static bool CheckIsSilent<T>(Span<T> buffer, T silenceThreshold) where T : unmanaged, INumber<T>
    {
        int sampleIndex = 0;
        if (Vector256.IsHardwareAccelerated && buffer.Length >= Vector256<T>.Count)
        {
            var silence256 = Vector256.Create(silenceThreshold);
            var buffer256 = MemoryMarshal.Cast<T, Vector256<T>>(buffer);
            for (int i = 0; i < buffer256.Length; i++)
            {
                if (Vector256.GreaterThanAny(Vector256.Abs(buffer256[i]), silence256))
                {
                    return false;
                }
            }

            sampleIndex = buffer256.Length * Vector256<T>.Count;
        }
        else if (Vector128.IsHardwareAccelerated && buffer.Length >= Vector128<T>.Count)
        {
            var silence128 = Vector128.Create(silenceThreshold);
            var buffer128 = MemoryMarshal.Cast<T, Vector128<T>>(buffer);
            for (int i = 0; i < buffer128.Length; i++)
            {
                if (Vector128.GreaterThanAny(Vector128.Abs(buffer128[i]), silence128))
                {
                    return false;
                }
            }

            sampleIndex = buffer128.Length * Vector128<T>.Count;
        }

        for (; sampleIndex < buffer.Length; sampleIndex++)
        {
            if (T.Abs(buffer[sampleIndex]) > silenceThreshold)
            {
                return false;
            }
        }
        return true;
    }
}