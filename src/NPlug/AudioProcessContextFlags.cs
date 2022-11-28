// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// Transport state and other flags.
/// </summary>
[Flags]
public enum AudioProcessContextFlags : uint
{
    /// <summary>
    /// currently playing
    /// </summary>
    Playing = 1 << 1,

    /// <summary>
    /// cycle is active
    /// </summary>
    CycleActive = 1 << 2,

    /// <summary>
    /// currently recording
    /// </summary>
    Recording = 1 << 3,

    /// <summary>
    /// systemTime contains valid information
    /// </summary>
    SystemTimeValid = 1 << 8,

    /// <summary>
    /// continousTimeSamples contains valid information
    /// </summary>
    ContTimeValid = 1 << 17,

    /// <summary>
    /// projectTimeMusic contains valid information
    /// </summary>
    ProjectTimeMusicValid = 1 << 9,

    /// <summary>
    /// barPositionMusic contains valid information
    /// </summary>
    BarPositionValid = 1 << 11,

    /// <summary>
    /// cycleStartMusic and barPositionMusic contain valid information
    /// </summary>
    CycleValid = 1 << 12,

    /// <summary>
    /// tempo contains valid information
    /// </summary>
    TempoValid = 1 << 10,

    /// <summary>
    /// timeSigNumerator and timeSigDenominator contain valid information
    /// </summary>
    TimeSigValid = 1 << 13,

    /// <summary>
    /// chord contains valid information
    /// </summary>
    ChordValid = 1 << 18,

    /// <summary>
    /// smpteOffset and frameRate contain valid information
    /// </summary>
    SmpteValid = 1 << 14,

    /// <summary>
    /// samplesToNextClock valid
    /// </summary>
    ClockValid = 1 << 15,
}