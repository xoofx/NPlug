// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// A chord is described with a key note, a root note and a chord mask
/// </summary>
public struct AudioChord
{
    /// <summary>
    /// key note in chord
    /// </summary>
    public byte KeyNote;

    /// <summary>
    /// lowest note in chord
    /// </summary>
    public byte RootNote;

    /// <summary>
    /// Bitmask of a chord. @n 1st bit set: minor second; 2nd bit set: major second, and so on. @n There is @b no bit for the keynote (root of the chord) because it is inherently always present. @n Examples:
    /// - XXXX 0000 0100 1000 (= 0x0048) -&gt; major chord
    /// - XXXX 0000 0100 0100 (= 0x0044) -&gt; minor chord
    /// - XXXX 0010 0100 0100 (= 0x0244) -&gt; minor chord with minor seventh
    /// </summary>
    public short ChordMask;
}

//public enum AudioChordMasks
//{
//    /// <summary>
//    /// mask for chordMask
//    /// </summary>
//    ChordMask = 0x0FFF,

//    /// <summary>
//    /// reserved for future use
//    /// </summary>
//    ReservedMask = 0xF000,
//}