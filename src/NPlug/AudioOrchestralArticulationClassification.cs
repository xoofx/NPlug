// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Classification of the note-on articulations of the MIDI 2.0 orchestral profile, used as the first index of
/// <see cref="AudioOrchestralArticulationVariations"/>.
/// </summary>
/// <remarks>
/// This is the equivalent of <c>Vst::NoteOnOrchestralArticulation::IInfo::Classification</c> (VST 3.8.1).
/// </remarks>
public enum AudioOrchestralArticulationClassification : uint
{
    /// <summary>
    /// Core sound: sustains and strikes.
    /// </summary>
    CoreSoundSustainAndStrikes = 0,

    /// <summary>
    /// Staccatos and shorts.
    /// </summary>
    StaccatosAndShorts = 1,

    /// <summary>
    /// Same-note trills and repeats.
    /// </summary>
    SameNoteTrillsRepeats = 2,

    /// <summary>
    /// Intervallic trills.
    /// </summary>
    IntervallicTrills = 3,

    /// <summary>
    /// Additional colors.
    /// </summary>
    AdditionalColors = 4,

    /// <summary>
    /// Pitch and dynamic gestures.
    /// </summary>
    PitchAndDynamicGestures = 5,

    /// <summary>
    /// Scales, runs and arpeggios.
    /// </summary>
    ScalesRunsAndArpeggios = 6,

    /// <summary>
    /// Effects and noises.
    /// </summary>
    EffectsAndNoises = 7,

    /// <summary>
    /// Reserved.
    /// </summary>
    Reserved1 = 8,

    /// <summary>
    /// Reserved.
    /// </summary>
    Reserved2 = 9,

    /// <summary>
    /// Custom note-on articulations 1.
    /// </summary>
    CustomNoteOn1 = 10,

    /// <summary>
    /// Custom note-on articulations 2.
    /// </summary>
    CustomNoteOn2 = 11,

    /// <summary>
    /// Custom note-on articulations 3.
    /// </summary>
    CustomNoteOn3 = 12,

    /// <summary>
    /// Custom note-on articulations 4.
    /// </summary>
    CustomNoteOn4 = 13,

    /// <summary>
    /// Custom note-on articulations 5.
    /// </summary>
    CustomNoteOn5 = 14,

    /// <summary>
    /// Custom note-on articulations 6.
    /// </summary>
    CustomNoteOn6 = 15,

    /// <summary>
    /// Reserved.
    /// </summary>
    Reserved3 = 16,
}
