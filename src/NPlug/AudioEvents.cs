// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// Audio event values accessible through <see cref="AudioEvent.Value"/>
/// </summary>
public static class AudioEvents
{

    /// <summary>
    /// Note-on event specific data. Used in @ref Event (union)
    /// </summary>
    /// <remarks>
    ///  vstEventGrpPitch uses the twelve-tone equal temperament tuning (12-TET).
    /// </remarks>
    public struct NoteOnEvent
    {
        /// <summary>
        /// channel index in event bus
        /// </summary>
        public short Channel;

        /// <summary>
        /// range [0, 127] = [C-2, G8] with A3=440Hz (12-TET: twelve-tone equal temperament)
        /// </summary>
        public short Pitch;

        /// <summary>
        /// 1.f = +1 cent, -1.f = -1 cent
        /// </summary>
        public float Tuning;

        /// <summary>
        /// range [0.0, 1.0]
        /// </summary>
        public float Velocity;

        /// <summary>
        /// in sample frames (optional, Note Off has to follow in any case!)
        /// </summary>
        public int Length;

        /// <summary>
        /// note identifier (if not available then -1)
        /// </summary>
        public int NoteId;
    }

    /// <summary>
    /// Note-off event specific data. Used in @ref Event (union)
    /// </summary>
    /// <remarks>
    ///  vstEventGrp
    /// </remarks>
    public struct NoteOffEvent
    {
        /// <summary>
        /// channel index in event bus
        /// </summary>
        public short Channel;

        /// <summary>
        /// range [0, 127] = [C-2, G8] with A3=440Hz (12-TET)
        /// </summary>
        public short Pitch;

        /// <summary>
        /// range [0.0, 1.0]
        /// </summary>
        public float Velocity;

        /// <summary>
        /// associated noteOn identifier (if not available then -1)
        /// </summary>
        public int NoteId;

        /// <summary>
        /// 1.f = +1 cent, -1.f = -1 cent
        /// </summary>
        public float Tuning;
    }


    /// <summary>
    /// Data event specific data. Used in @ref Event (union)
    /// </summary>
    /// <remarks>
    ///  vstEventGrp
    /// </remarks>
    public unsafe struct DataEvent
    {
        /// <summary>
        /// size in bytes of the data block bytes
        /// </summary>
        private int _size;

        /// <summary>
        /// type of this data block (see @ref DataTypes)
        /// </summary>
        public AudioDataEventKind Kind;

        /// <summary>
        /// pointer to the data block
        /// </summary>
        private byte* _bytes;

        public Span<byte> Buffer => new(_bytes, _size);
    }

    public enum AudioDataEventKind : uint
    {
        /// <summary>
        /// for MIDI system exclusive message
        /// </summary>
        MidiSysEx = 0,
    }

    public struct PolyPressureEvent
    {
        /// <summary>
        /// channel index in event bus
        /// </summary>
        public short Channel;

        /// <summary>
        /// range [0, 127] = [C-2, G8] with A3=440Hz
        /// </summary>
        public short Pitch;

        /// <summary>
        /// range [0.0, 1.0]
        /// </summary>
        public float Pressure;

        /// <summary>
        /// event should be applied to the noteId (if not -1)
        /// </summary>
        public int NoteId;
    }

    public struct NoteExpressionValueEvent
    {
        /// <summary>
        /// see @ref NoteExpressionTypeID
        /// </summary>
        public uint TypeId;

        /// <summary>
        /// associated note identifier to apply the change
        /// </summary>
        public int NoteId;

        /// <summary>
        /// normalized value [0.0, 1.0].
        /// </summary>
        public double Value;
    }

    /// <summary>
    /// Note Expression Text event. Used in Event (union)
    /// A Expression event affects one single playing note.
    /// </summary>
    public unsafe struct NoteExpressionTextEvent
    {
        /// <summary>
        /// see @ref NoteExpressionTypeID (kTextTypeID or kPhoneticTypeID)
        /// </summary>
        public uint TypeId;

        /// <summary>
        /// associated note identifier to apply the change
        /// </summary>
        public int NoteId;

        /// <summary>
        /// the number of characters (TChar) between the beginning of text and the terminating
        /// null character (without including the terminating null character itself)
        /// </summary>
        private int _textLen;

        /// <summary>
        /// UTF-16, null terminated
        /// </summary>
        private char* _text;


        /// <summary>
        /// Gets the text associated.
        /// </summary>
        /// <returns></returns>
        public string GetText() => new(*_text, _textLen);
    }

    public unsafe struct ChordEvent
    {
        /// <summary>
        /// range [0, 127] = [C-2, G8] with A3=440Hz
        /// </summary>
        public short Root;

        /// <summary>
        /// range [0, 127] = [C-2, G8] with A3=440Hz
        /// </summary>
        public short BassNote;

        /// <summary>
        /// root is bit 0
        /// </summary>
        public short Mask;

        /// <summary>
        /// the number of characters (TChar) between the beginning of text and the terminating
        /// null character (without including the terminating null character itself)
        /// </summary>
        private ushort _textLen;

        /// <summary>
        /// UTF-16, null terminated Hosts Chord Name
        /// </summary>
        private char* _text;

        /// <summary>
        /// Gets the text associated.
        /// </summary>
        /// <returns></returns>
        public string GetText() => new(*_text, _textLen);
    }

    public unsafe struct ScaleEvent
    {
        /// <summary>
        /// range [0, 127] = root Note/Transpose Factor
        /// </summary>
        public short Root;

        /// <summary>
        /// Bit 0 =  C,  Bit 1 = C#, ... (0x5ab5 = Major Scale)
        /// </summary>
        public short Mask;

        /// <summary>
        /// the number of characters (TChar) between the beginning of text and the terminating
        /// null character (without including the terminating null character itself)
        /// </summary>
        private ushort _textLen;

        /// <summary>
        /// UTF-16, null terminated, Hosts Scale Name
        /// </summary>
        private char* _text;

        /// <summary>
        /// Gets the text associated.
        /// </summary>
        /// <returns></returns>
        public string GetText() => new(*_text, _textLen);
    }

    public struct LegacyMIDICCOutEvent
    {
        /// <summary>
        /// see enum ControllerNumbers [0, 255]
        /// </summary>
        public byte ControlNumber;

        /// <summary>
        /// channel index in event bus [0, 15]
        /// </summary>
        public sbyte Channel;

        /// <summary>
        /// value of Controller [0, 127]
        /// </summary>
        public sbyte Value;

        /// <summary>
        /// [0, 127] used for pitch bend (kPitchBend) and polyPressure (kCtrlPolyPressure)
        /// </summary>
        public sbyte Value2;
    }
}