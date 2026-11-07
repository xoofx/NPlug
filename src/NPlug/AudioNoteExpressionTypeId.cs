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
    /// start of the MIDI 2.0 assignable per-note controllers (VST 3.8.1)
    /// </summary>
    Midi2AssignablePerNoteControllerStart = 201000,
    /// <summary>
    /// end of the MIDI 2.0 assignable per-note controllers (VST 3.8.1)
    /// </summary>
    Midi2AssignablePerNoteControllerEnd = 201255,
    /// <summary>
    /// start of the MIDI 2.0 registered per-note controllers (VST 3.8.1)
    /// </summary>
    Midi2RegisteredPerNoteControllerStart = 202000,
    /// <summary>
    /// MIDI 2.0 registered per-note controller 0x0C: playing position (VST 3.8.1)
    /// </summary>
    Midi2RegisteredPerNoteControllerPlayingPosition = Midi2RegisteredPerNoteControllerStart + 0x0C,
    /// <summary>
    /// end of the MIDI 2.0 registered per-note controllers (VST 3.8.1)
    /// </summary>
    Midi2RegisteredPerNoteControllerEnd = 202255,
    /// <summary>
    /// start of the MIDI 2.0 note-on attributes (VST 3.8.1)
    /// </summary>
    Midi2NoteOnAttributeStart = 203000,
    /// <summary>
    /// start of the note-on articulations of the MIDI 2.0 orchestral profile (VST 3.8.1)
    /// </summary>
    Midi2NoteOnOrchestralArticulationStart = Midi2NoteOnAttributeStart + 0x10,
    /// <summary>
    /// end of the note-on articulations of the MIDI 2.0 orchestral profile (VST 3.8.1)
    /// </summary>
    Midi2NoteOnOrchestralArticulationEnd = Midi2NoteOnAttributeStart + 0x1F,
    /// <summary>
    /// end of the MIDI 2.0 note-on attributes (VST 3.8.1)
    /// </summary>
    Midi2NoteOnAttributeEnd = 203255,
    /// <summary>
    /// start of the MIDI 2.0 note-off attributes (VST 3.8.1)
    /// </summary>
    Midi2NoteOffAttributeStart = 204000,
    /// <summary>
    /// note-off articulation of the MIDI 2.0 orchestral profile (VST 3.8.1)
    /// </summary>
    Midi2NoteOffOrchestralArticulation = Midi2NoteOffAttributeStart + 0x10,
    /// <summary>
    /// end of the MIDI 2.0 note-off attributes (VST 3.8.1)
    /// </summary>
    Midi2NoteOffAttributeEnd = 204255,
    /// <summary>
    /// indicates an invalid note expression type
    /// </summary>
    InvalidTypeID = 0xFFFFFFFF,
}