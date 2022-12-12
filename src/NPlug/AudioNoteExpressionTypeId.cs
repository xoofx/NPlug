// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// NoteExpressionTypeIDs describes the type of the note expression.
/// VST predefines some types like volume, pan, tuning by defining their ranges and curves.
/// Used by NoteExpressionEvent::typeId and NoteExpressionTypeID::typeId
/// </summary>
/// <seealso cref="AudioNoteExpressionTypeInfo"/>
public enum AudioNoteExpressionTypeId : uint
{
    /// <summary>
    /// Volume, plain range [0 = -oo , 0.25 = 0dB, 0.5 = +6dB, 1 = +12dB]: plain = 20 * log (4 * norm)
    /// </summary>
    Volume = 0,

    /// <summary>
    /// Panning (L-R), plain range [0 = left, 0.5 = center, 1 = right]
    /// </summary>
    Pan,

    /// <summary>
    /// Tuning, plain range [0 = -120.0 (ten octaves down), 0.5 none, 1 = +120.0 (ten octaves up)]
    /// plain = 240 * (norm - 0.5) and norm = plain / 240 + 0.5
    /// oneOctave is 12.0 / 240.0; oneHalfTune = 1.0 / 240.0;
    /// </summary>
    Tuning,

    /// <summary>
    /// Vibrato
    /// </summary>
    Vibrato,

    /// <summary>
    /// Expression
    /// </summary>
    Expression,

    /// <summary>
    /// Brightness
    /// </summary>
    Brightness,

    /// <summary>
    /// See NoteExpressionTextEvent
    /// </summary>
    Text,

    /// <summary>
    /// TODO:
    /// </summary>
    Phoneme,

    /// <summary>
    /// start of custom note expression type ids
    /// </summary>
    CustomStart = 100000,

    /// <summary>
    /// end of custom note expression type ids
    /// </summary>
    CustomEnd = 200000,

    /// <summary>
    /// indicates an invalid note expression type
    /// </summary>
    InvalidTypeID = 0xFFFFFFFF,
}