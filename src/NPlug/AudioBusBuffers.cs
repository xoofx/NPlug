// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
// ReSharper disable UnassignedReadonlyField

namespace NPlug;

public readonly unsafe ref struct AudioBusBuffers
{
    /// <summary>
    /// Number of audio channels in bus.
    /// </summary>
    public readonly int ChannelCount;

    /// <summary>
    /// Bitset of silence state per channel.
    /// </summary>
    public readonly ulong SilenceFlags;

    private readonly void** _channelBuffers;

    public bool IsChannelSilence(int index) => (SilenceFlags & (1UL << index)) != 0;

    public Span<float> AsFloat32(in AudioProcessSetupData setupData, in AudioProcessData processData, int channelIndex)
    {
        if (setupData.SampleSize != AudioSampleSize.Float32) throw new InvalidOperationException($"Expecting 32-bit samples but getting {setupData.SampleSize}");
        if ((uint)channelIndex >= (uint)ChannelCount) throw new ArgumentException($"Invalid Channel Index {channelIndex}", nameof(channelIndex));
        return new Span<float>((float*)_channelBuffers[channelIndex], processData.SampleCount);
    }

    public Span<double> AsFloat64(in AudioProcessSetupData setupData, in AudioProcessData processData, int channelIndex)
    {
        if (setupData.SampleSize != AudioSampleSize.Float64) throw new InvalidOperationException($"Expecting 64-bit samples but getting {setupData.SampleSize}");
        if ((uint)channelIndex >= (uint)ChannelCount) throw new ArgumentException($"Invalid Channel Index {channelIndex}", nameof(channelIndex));
        return new Span<double>((double*)_channelBuffers[channelIndex], processData.SampleCount);
    }
}