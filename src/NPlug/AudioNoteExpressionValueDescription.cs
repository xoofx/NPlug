// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug;

/// <summary>
/// Description of a Note Expression Type
/// This structure is part of the <see cref="AudioNoteExpressionTypeInfo"/> structure, it describes for given <see cref="AudioNoteExpressionTypeId"/> its default value
/// (for example 0.5 for a <see cref="AudioNoteExpressionTypeId.Tuning"/> (IsBipolar: centered)), its minimum and maximum (for predefined <see cref="AudioNoteExpressionTypeId"/> the full range is predefined too)
/// and a stepCount when the given <see cref="AudioNoteExpressionTypeId"/> is limited to discrete values (like on/off state).
/// </summary>
/// <seealso cref="AudioNoteExpressionTypeInfo"/>
public readonly record struct AudioNoteExpressionValueDescription
{
    /// <summary>
    /// default normalized value [0,1]
    /// </summary>
    public double DefaultValue { get; init; }

    /// <summary>
    /// minimum normalized value [0,1]
    /// </summary>
    public double Minimum { get; init; }

    /// <summary>
    /// maximum normalized value [0,1]
    /// </summary>
    public double Maximum { get; init; }

    /// <summary>
    /// number of discrete steps (0: continuous, 1: toggle, discrete value otherwise - see @ref vst3ParameterIntro)
    /// </summary>
    public int StepCount { get; init; }
}