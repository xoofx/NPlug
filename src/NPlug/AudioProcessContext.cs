// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

// ReSharper disable UnassignedReadonlyField
namespace NPlug;

public readonly struct AudioProcessContext
{
    /// <summary>
    /// a combination of the values from @ref StatesAndFlags
    /// </summary>
    public readonly AudioProcessContextFlags Flags;

    /// <summary>
    /// current sample rate					(always valid)
    /// </summary>
    public readonly double SampleRate;

    /// <summary>
    /// project time in samples				(always valid)
    /// </summary>
    public readonly long ProjectTimeSamples;

    /// <summary>
    /// system time in nanoseconds					(optional)
    /// </summary>
    public readonly long SystemTime;

    /// <summary>
    /// project time, without loop					(optional)
    /// </summary>
    public readonly long ContinuousTimeSamples;

    /// <summary>
    /// musical position in quarter notes (1.0 equals 1 quarter note) (optional)
    /// </summary>
    public readonly double ProjectTimeMusic;

    /// <summary>
    /// last bar start position, in quarter notes	(optional)
    /// </summary>
    public readonly double BarPositionMusic;

    /// <summary>
    /// cycle start in quarter notes				(optional)
    /// </summary>
    public readonly double CycleStartMusic;

    /// <summary>
    /// cycle end in quarter notes					(optional)
    /// </summary>
    public readonly double CycleEndMusic;

    /// <summary>
    /// tempo in BPM (Beats Per Minute)			(optional)
    /// </summary>
    public readonly double Tempo;

    /// <summary>
    /// time signature numerator (e.g. 3 for 3/4)	(optional)
    /// </summary>
    public readonly int TimeSigNumerator;

    /// <summary>
    /// time signature denominator (e.g. 4 for 3/4) (optional)
    /// </summary>
    public readonly int TimeSigDenominator;

    /// <summary>
    /// musical info								(optional)
    /// </summary>
    public readonly AudioChord Chord;

    /// <summary>
    /// SMPTE (sync) offset in subframes (1/80 of frame) (optional)
    /// </summary>
    public readonly int SmpteOffsetSubFrames;

    /// <summary>
    /// frame rate									(optional)
    /// </summary>
    public readonly AudioFrameRate FrameRate;

    /// <summary>
    /// MIDI Clock Resolution (24 Per Quarter Note), can be negative (nearest) (optional)
    /// </summary>
    public readonly int SamplesToNextClock;
}