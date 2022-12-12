// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// The structure describing a note expression supported by the plug-in.
/// This structure is used by the method <see cref="IAudioControllerNoteExpression.GetNoteExpressionInfo"/>.
/// </summary>
/// <seealso cref="IAudioControllerNoteExpression"/>
public sealed record AudioNoteExpressionTypeInfo
{
    /// <summary>
    /// unique identifier of this note Expression type
    /// </summary>
    public required AudioNoteExpressionTypeId TypeId { get; init; }

    /// <summary>
    /// note Expression type title (e.g. "Volume")
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// note Expression type short title (e.g. "Vol")
    /// </summary>
    public string ShortTitle { get; init; } = string.Empty;

    /// <summary>
    /// note Expression type unit (e.g. "dB")
    /// </summary>
    public string Units { get; init; } = string.Empty;

    /// <summary>
    /// id of unit this NoteExpression belongs to (see @ref vst3Units), in order to sort the note expression, it is possible to use unitId like for parameters. -1 means no unit used.
    /// </summary>
    public AudioUnitId UnitId { get; init; } = AudioUnitId.NoUnits;

    /// <summary>
    /// value description see @ref NoteExpressionValueDescription
    /// </summary>
    public AudioNoteExpressionValueDescription ValueDescription { get; init; }

    /// <summary>
    /// optional associated parameter ID (for mapping from note expression to global (using the parameter automation for example) and back). Only used when kAssociatedParameterIDValid is set in flags.
    /// </summary>
    public AudioParameterId AssociatedParameterId { get; init; }

    /// <summary>
    /// NoteExpressionTypeFlags (see below)
    /// </summary>
    public AudioNoteExpressionTypeFlags Flags { get; init; }
}