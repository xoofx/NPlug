// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public enum AudioEventKind : ushort
{
    /// <summary>
    /// is @ref NoteOnEvent
    /// </summary>
    NoteOn = 0,

    /// <summary>
    /// is @ref NoteOffEvent
    /// </summary>
    NoteOff = 1,

    /// <summary>
    /// is @ref DataEvent
    /// </summary>
    Data = 2,

    /// <summary>
    /// is @ref PolyPressureEvent
    /// </summary>
    PolyPressure = 3,

    /// <summary>
    /// is @ref NoteExpressionValueEvent
    /// </summary>
    NoteExpressionValue = 4,

    /// <summary>
    /// is @ref NoteExpressionTextEvent
    /// </summary>
    NoteExpressionText = 5,

    /// <summary>
    /// is @ref ChordEvent
    /// </summary>
    Chord = 6,

    /// <summary>
    /// is @ref ScaleEvent
    /// </summary>
    Scale = 7,

    /// <summary>
    /// is @ref LegacyMIDICCOutEvent
    /// </summary>
    LegacyMIDICCOut = 65535,
}