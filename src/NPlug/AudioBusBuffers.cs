// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
// ReSharper disable UnassignedReadonlyField

namespace NPlug;

/// <summary>
/// The buffers for a specific bus, used in <see cref="AudioBusData"/> while processing audio via <see cref="IAudioProcessor.Process"/>.
/// </summary>
public unsafe ref struct AudioBusBuffers
{
    /// <summary>
    /// Number of audio channels in bus.
    /// </summary>
    public readonly int ChannelCount;

    /// <summary>
    /// Bitset of silence state per channel.
    /// </summary>
    public ulong SilenceFlags;

    // internal pointer to buffers. Use GetChannelSpanAsBytes / GetChannelSpanAsFloat32 / GetChannelSpanAsFloat64 methods.
    private readonly void** _channelBuffers;
    
    /// <summary>
    /// Mark a specific channel as silence or not.
    /// </summary>
    /// <param name="channelIndex">The index of the channel.</param>
    /// <param name="silence"><c>true</c> to mark the channel as silence.</param>
    public void SetChannelSilence(int channelIndex, bool silence)
    {
        if (silence)
        {
            SilenceFlags |= (1UL << channelIndex);
        }
        else
        {
            SilenceFlags &= ~(1UL << channelIndex);
        }
    }

    /// <summary>
    /// Checks whether the specified channel is silenced.
    /// </summary>
    /// <param name="channelIndex">The index of the channel.</param>
    /// <returns><c>true</c> if the channel is silenced.</returns>
    public bool IsChannelSilence(int channelIndex) => (SilenceFlags & (1UL << channelIndex)) != 0;

    /// <summary>
    /// Safely gets the buffer associated with the current sampling rate and sample size.
    /// </summary>
    /// <param name="setupData">The processing setup data initialized by <see cref="IAudioProcessor.SetupProcessing"/>.</param>
    /// <param name="processData">The processing data provided during <see cref="IAudioProcessor.Process"/>.</param>
    /// <param name="channelIndex">The index of the channel.</param>
    /// <returns>A span of the audio buffer.</returns>
    /// <exception cref="ArgumentException">If the index is out of range.</exception>
    public Span<byte> GetChannelSpanAsBytes(in AudioProcessSetupData setupData, in AudioProcessData processData, int channelIndex)
    {
        if ((uint)channelIndex >= (uint)ChannelCount) throw new ArgumentException($"Invalid Channel Index {channelIndex}", nameof(channelIndex));
        var size = setupData.SampleSize == AudioSampleSize.Float32 ? 4 : 8;
        return new Span<byte>((byte*)_channelBuffers[channelIndex], size * processData.SampleCount);
    }

    /// <summary>
    /// Safely gets the buffer associated with the current sampling rate and sample size.
    /// </summary>
    /// <param name="setupData">The processing setup data initialized by <see cref="IAudioProcessor.SetupProcessing"/>.</param>
    /// <param name="processData">The processing data provided during <see cref="IAudioProcessor.Process"/>.</param>
    /// <param name="channelIndex">The index of the channel.</param>
    /// <returns>A span of the audio buffer.</returns>
    /// <exception cref="ArgumentException">If the index is out of range or the sample size is not Float32.</exception>
    public Span<float> GetChannelSpanAsFloat32(in AudioProcessSetupData setupData, in AudioProcessData processData, int channelIndex)
    {
        if (setupData.SampleSize != AudioSampleSize.Float32) throw new InvalidOperationException($"Expecting 32-bit samples but getting {setupData.SampleSize}");
        if ((uint)channelIndex >= (uint)ChannelCount) throw new ArgumentException($"Invalid Channel Index {channelIndex}", nameof(channelIndex));
        return new Span<float>((float*)_channelBuffers[channelIndex], processData.SampleCount);
    }

    /// <summary>
    /// Safely gets the buffer associated with the current sampling rate and sample size.
    /// </summary>
    /// <param name="setupData">The processing setup data initialized by <see cref="IAudioProcessor.SetupProcessing"/>.</param>
    /// <param name="processData">The processing data provided during <see cref="IAudioProcessor.Process"/>.</param>
    /// <param name="channelIndex">The index of the channel.</param>
    /// <returns>A span of the audio buffer.</returns>
    /// <exception cref="ArgumentException">If the index is out of range or the sample size is not Float64.</exception>
    public Span<double> GetChannelSpanAsFloat64(in AudioProcessSetupData setupData, in AudioProcessData processData, int channelIndex)
    {
        if (setupData.SampleSize != AudioSampleSize.Float64) throw new InvalidOperationException($"Expecting 64-bit samples but getting {setupData.SampleSize}");
        if ((uint)channelIndex >= (uint)ChannelCount) throw new ArgumentException($"Invalid Channel Index {channelIndex}", nameof(channelIndex));
        return new Span<double>((double*)_channelBuffers[channelIndex], processData.SampleCount);
    }
}