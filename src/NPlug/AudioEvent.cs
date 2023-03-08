// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace NPlug;

/// <summary>
/// Describes an audio event.
/// </summary>
public struct AudioEvent
{
    /// <summary>
    /// event bus index
    /// </summary>
    public int BusIndex;

    /// <summary>
    /// sample frames related to the current block start sample position
    /// </summary>
    public int SampleOffset;

    /// <summary>
    /// position in project
    /// </summary>
    public double Position;

    /// <summary>
    /// combination of @ref EventFlags
    /// </summary>
    public AudioEventFlags Flags;

    /// <summary>
    /// a value from @ref EventTypes
    /// </summary>
    public AudioEventKind Kind;

    /// <summary>
    /// The value of this event.
    /// </summary>
    public AudioEventValue Value;

    /// <summary>
    /// The value of an <see cref="AudioEvent"/> that depends on the <see cref="AudioEvent.Kind"/>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct AudioEventValue
    {
        /// <summary>
        /// type == kNoteOnEvent
        /// </summary>
        [FieldOffset(0)]
        public AudioEvents.NoteOnEvent NoteOn;

        /// <summary>
        /// type == kNoteOffEvent
        /// </summary>
        [FieldOffset(0)]
        public AudioEvents.NoteOffEvent NoteOff;

        /// <summary>
        /// type == kDataEvent
        /// </summary>
        [FieldOffset(0)]
        public AudioEvents.DataEvent Data;

        /// <summary>
        /// type == kPolyPressureEvent
        /// </summary>
        [FieldOffset(0)]
        public AudioEvents.PolyPressureEvent PolyPressure;

        /// <summary>
        /// type == kNoteExpressionValueEvent
        /// </summary>
        [FieldOffset(0)]
        public AudioEvents.NoteExpressionValueEvent NoteExpressionValue;

        /// <summary>
        /// type == kNoteExpressionTextEvent
        /// </summary>
        [FieldOffset(0)]
        public AudioEvents.NoteExpressionTextEvent NoteExpressionText;

        /// <summary>
        /// type == kChordEvent
        /// </summary>
        [FieldOffset(0)]
        public AudioEvents.ChordEvent Chord;

        /// <summary>
        /// type == kScaleEvent
        /// </summary>
        [FieldOffset(0)]
        public AudioEvents.ScaleEvent Scale;

        /// <summary>
        /// type == kLegacyMIDICCOutEvent
        /// </summary>
        [FieldOffset(0)]
        public AudioEvents.LegacyMIDICCOutEvent MidiCCOut;
    }
}