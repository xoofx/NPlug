// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug.SimpleDelay;

public class SimpleDelayModel : AudioProcessorModel
{
    public SimpleDelayModel() : base("NPlug.SimpleDelay")
    {
        AddByPassParameter();
        Delay = AddParameter(new AudioParameter("Delay", units: "sec", defaultNormalizedValue: 1.0));
    }

    public AudioParameter Delay { get; }
}
