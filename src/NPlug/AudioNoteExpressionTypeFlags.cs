// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
using System;

namespace NPlug;

/// <summary>
/// Flags defined for <see cref="AudioNoteExpressionTypeInfo.Flags"/>.
/// </summary>
[Flags]
public enum AudioNoteExpressionTypeFlags
{
    /// <summary>
    /// None flags.
    /// </summary>
    None = 0,

    /// <summary>
    /// event is bipolar (centered), otherwise unipolar
    /// </summary>
    IsBipolar = 1 << 0,

    /// <summary>
    /// event occurs only one time for its associated note (at begin of the noteOn)
    /// </summary>
    IsOneShot = 1 << 1,

    /// <summary>
    /// This note expression will apply an absolute change to the sound (not relative (offset))
    /// </summary>
    IsAbsolute = 1 << 2,

    /// <summary>
    /// indicates that the associatedParameterID is valid and could be used
    /// </summary>
    AssociatedParameterIdValid = 1 << 3,
}