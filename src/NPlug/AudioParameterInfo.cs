// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using NPlug.IO;

namespace NPlug;

/// <summary>
/// Controller Parameter Info.
/// A parameter info describes a parameter of the controller.
/// The id must always be the same for a parameter as this uniquely identifies the parameter.
/// </summary>
public sealed record AudioParameterInfo(AudioParameterId Id, string Title)
{
    /// <summary>
    /// unique identifier of this parameter (named tag too)
    /// </summary>
    public AudioParameterId Id { get; init; } = Id;

    /// <summary>
    /// parameter title (e.g. "Volume")
    /// </summary>
    public string Title { get; init; } = Title;

    /// <summary>
    /// parameter shortTitle (e.g. "Vol")
    /// </summary>
    public string ShortTitle { get; init; } = string.Empty;

    /// <summary>
    /// parameter unit (e.g. "dB")
    /// </summary>
    public string Units { get; init; } = string.Empty;

    /// <summary>
    /// number of discrete steps (0: continuous, 1: toggle, discrete value otherwise 
    /// (corresponding to max - min, for example: 127 for a min = 0 and a max = 127) - see @ref vst3ParameterIntro)
    /// </summary>
    public int StepCount { get; init; }

    /// <summary>
    /// default normalized value [0,1] (in case of discrete value: defaultNormalizedValue = defDiscreteValue / stepCount)
    /// </summary>
    public double DefaultNormalizedValue { get; init; }

    /// <summary>
    /// id of unit this parameter belongs to (see @ref vst3Units)
    /// </summary>
    public AudioUnitId UnitId { get; init; }

    /// <summary>
    /// Flags for the parameter
    /// </summary>
    public AudioParameterFlags Flags { get; init; } = AudioParameterFlags.CanAutomate;
}