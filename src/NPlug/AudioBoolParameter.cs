// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public class AudioBoolParameter : AudioParameter
{
    public AudioBoolParameter(AudioParameterInfo info) : base(info)
    {
        StepCount = 1;
    }

    public AudioBoolParameter(string title, int id = 0, string? units = null, string? shortTitle = null, bool defaultValue = false, AudioParameterFlags flags = AudioParameterFlags.CanAutomate) : base(title, units, id, shortTitle, 1, defaultValue ? 1.0 : 0.0, flags)
    {
    }

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