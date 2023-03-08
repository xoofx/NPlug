// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Audio processing setup.
/// </summary>
public readonly record struct AudioProcessSetupData
{
    /// <summary>
    /// Creates a new instance of this struct.
    /// </summary>
    public AudioProcessSetupData(AudioProcessMode processMode, AudioSampleSize sampleSize, int maxSamplesPerBlock, double sampleRate)
    {
        ProcessMode = processMode;
        SampleSize = sampleSize;
        MaxSamplesPerBlock = maxSamplesPerBlock;
        SampleRate = sampleRate;
    }
    
    /// <summary>
    /// The process mode.
    /// </summary>
    public readonly AudioProcessMode ProcessMode;

    /// <summary>
    /// SampleSize.
    /// </summary>
    public readonly AudioSampleSize SampleSize;

    /// <summary>
    /// maximum number of samples per audio block
    /// </summary>
    public readonly int MaxSamplesPerBlock;

    /// <summary>
    /// sample rate
    /// </summary>
    public readonly double SampleRate;
}