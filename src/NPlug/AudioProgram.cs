// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Generic;

namespace NPlug;

public class AudioProgram
{
    public AudioProgram(string name)
    {
        Name = name;
        PitchNames = new Dictionary<short, string>();
    }

    public string Name { get; set; }
    
    public Dictionary<short, string> PitchNames { get; }
}