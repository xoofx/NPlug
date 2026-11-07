// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Unit of an <see cref="AudioTransportPosition"/>.
/// </summary>
public enum AudioTransportPositionKind
{
    /// <summary>
    /// Absolute position in samples.
    /// </summary>
    Samples = 0,

    /// <summary>
    /// Musical position in quarter notes (PPQ).
    /// </summary>
    QuarterNotes = 1,
}

/// <summary>
/// A transport position given with an <see cref="AudioTransportAction"/>, in samples or in quarter notes.
/// </summary>
/// <remarks>
/// This is the equivalent of <c>Vst::TransportPosition</c> (VST 3.8.1). Hosts may honor only one of the two units.
/// </remarks>
public readonly record struct AudioTransportPosition
{
    private AudioTransportPosition(AudioTransportPositionKind kind, long samples, double quarterNotes)
    {
        Kind = kind;
        Samples = samples;
        QuarterNotes = quarterNotes;
    }

    /// <summary>
    /// Gets the unit of this position.
    /// </summary>
    public AudioTransportPositionKind Kind { get; }

    /// <summary>
    /// Gets the absolute position in samples, valid when <see cref="Kind"/> is <see cref="AudioTransportPositionKind.Samples"/>.
    /// </summary>
    public long Samples { get; }

    /// <summary>
    /// Gets the musical position in quarter notes, valid when <see cref="Kind"/> is <see cref="AudioTransportPositionKind.QuarterNotes"/>.
    /// </summary>
    public double QuarterNotes { get; }

    /// <summary>
    /// Creates a position in samples.
    /// </summary>
    /// <param name="samples">The absolute position in samples.</param>
    /// <returns>The position.</returns>
    public static AudioTransportPosition FromSamples(long samples) => new(AudioTransportPositionKind.Samples, samples, 0.0);

    /// <summary>
    /// Creates a musical position in quarter notes (PPQ).
    /// </summary>
    /// <param name="quarterNotes">The position in quarter notes.</param>
    /// <returns>The position.</returns>
    public static AudioTransportPosition FromQuarterNotes(double quarterNotes) => new(AudioTransportPositionKind.QuarterNotes, 0, quarterNotes);
}
