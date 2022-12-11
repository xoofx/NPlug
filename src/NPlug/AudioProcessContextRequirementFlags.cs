// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// Requirements for the content of <see cref="AudioProcessContext"/> when processing the audio.
/// </summary>
[Flags]
public enum AudioProcessContextRequirementFlags
{
    /// <summary>
    /// The audio processor does not require anything.
    /// </summary>
    None = 0,

    /// <summary>
    /// kSystemTimeValid
    /// </summary>
    NeedSystemTime = 1 << 0,

    /// <summary>
    /// kContTimeValid
    /// </summary>
    NeedContinousTimeSamples = 1 << 1,

    /// <summary>
    /// kProjectTimeMusicValid
    /// </summary>
    NeedProjectTimeMusic = 1 << 2,

    /// <summary>
    /// kBarPositionValid
    /// </summary>
    NeedBarPositionMusic = 1 << 3,

    /// <summary>
    /// kCycleValid
    /// </summary>
    NeedCycleMusic = 1 << 4,

    /// <summary>
    /// kClockValid
    /// </summary>
    NeedSamplesToNextClock = 1 << 5,

    /// <summary>
    /// kTempoValid
    /// </summary>
    NeedTempo = 1 << 6,

    /// <summary>
    /// kTimeSigValid
    /// </summary>
    NeedTimeSignature = 1 << 7,

    /// <summary>
    /// kChordValid
    /// </summary>
    NeedChord = 1 << 8,

    /// <summary>
    /// kSmpteValid
    /// </summary>
    NeedFrameRate = 1 << 9,

    /// <summary>
    /// kPlaying, kCycleActive, kRecording
    /// </summary>
    NeedTransportState = 1 << 10,
}