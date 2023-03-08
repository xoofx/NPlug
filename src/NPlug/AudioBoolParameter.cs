// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// A boolean <see cref="AudioParameter"/> that maps <c>true</c> to 1.0 and false to 0.0.
/// </summary>
public class AudioBoolParameter : AudioParameter
{
    /// <summary>
    /// Creates a new instance of this parameter.
    /// </summary>
    public AudioBoolParameter(AudioParameterInfo info) : base(info)
    {
        StepCount = 1;
    }

    /// <summary>
    /// Creates a new instance of this parameter.
    /// </summary>
    public AudioBoolParameter(string title, int id = 0, string? units = null, string? shortTitle = null, bool defaultValue = false, AudioParameterFlags flags = AudioParameterFlags.CanAutomate) : base(title, units, id, shortTitle, 1, defaultValue ? 1.0 : 0.0, flags)
    {
    }

    /// <summary>
    /// Gets or sets the value of this parameter.
    /// </summary>
    public bool Value
    {
        get
        {
            return NormalizedValue > 0.5;
        }
        set
        {
            NormalizedValue = value ? 1.0 : 0.0;
        }
    }
}