// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Numerics;
using NPlug.Helpers;
using NUnit.Framework;

namespace NPlug.Tests;

/// <summary>
/// <see cref="AudioHelper.CheckIsSilent{T}"/> must see every sample, whatever the buffer length and the SIMD width
/// (Vector256 with AVX2, Vector128 with SSE only, scalar remainder), and compare magnitudes (negative samples too).
/// </summary>
public class TestAudioHelper
{
    [Test]
    public void EverySampleIsCheckedFloat() => CheckAllPositions(0.000132184039f, 0.5f);

    [Test]
    public void EverySampleIsCheckedDouble() => CheckAllPositions(0.000132184039, 0.5);

    private static void CheckAllPositions<T>(T threshold, T loud) where T : unmanaged, INumber<T>
    {
        T quiet = threshold / T.CreateChecked(2);
        for (int length = 0; length <= 40; length++)
        {
            var buffer = new T[length];
            for (int i = 0; i < length; i++) buffer[i] = (i & 1) == 0 ? quiet : -quiet;
            Assert.IsTrue(AudioHelper.CheckIsSilent<T>(buffer, threshold), $"quiet buffer, length {length}");

            for (int position = 0; position < length; position++)
            {
                foreach (var value in new[] { loud, -loud })
                {
                    var saved = buffer[position];
                    buffer[position] = value;
                    Assert.IsFalse(AudioHelper.CheckIsSilent<T>(buffer, threshold), $"{typeof(T).Name} length {length}, sample {value} at {position}");
                    buffer[position] = saved;
                }
            }
        }
    }
}
