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
        bool isChannelSilent = true;
        int sampleIndex = 0;
        if (Vector256.IsHardwareAccelerated)
        {
            if (buffer.Length >= Vector256<T>.Count)
            {
                var silence256 = Vector256.Create<T>(silenceThreshold);
                var buffer256 = MemoryMarshal.Cast<T, Vector256<T>>(buffer);
                for (; sampleIndex < buffer256.Length; sampleIndex++)
                {
                    if (Vector256.GreaterThanAny(Vector256.Abs(buffer256[sampleIndex]), silence256))
                    {
                        isChannelSilent = false;
                        break;
                    }
                }

                sampleIndex *= Vector256<T>.Count;
            }
        }
        else if (Vector128.IsHardwareAccelerated)
        {
            if (buffer.Length >= Vector128<T>.Count)
            {
                var silence128 = Vector128.Create(silenceThreshold);
                var buffer128 = MemoryMarshal.Cast<T, Vector128<T>>(buffer);
                for (; sampleIndex < buffer128.Length; sampleIndex++)
                {
                    if (Vector128.GreaterThanAny(Vector128.Abs(buffer128[sampleIndex]), silence128))
                    {
                        isChannelSilent = false;
                        break;
                    }
                }

                sampleIndex *= Vector256<T>.Count;
            }
        }

        for (; sampleIndex < buffer.Length; sampleIndex++)
        {
            if (buffer[sampleIndex] > silenceThreshold)
            {
                isChannelSilent = false;
                break;
            }
        }
        return isChannelSilent;
    }
}