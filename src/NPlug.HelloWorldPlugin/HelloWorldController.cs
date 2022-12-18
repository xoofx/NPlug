// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public class HelloWorldController : AudioController<HelloWorldModel>
{
    public static readonly Guid ClassId = new("B4752A4E-C4FC-40BB-9FCE-42178135A69E");

    public HelloWorldController()
    {
    }

    protected override bool Initialize(AudioHostApplication host)
    {
        SetMidiCCMapping(AudioMidiControllerNumber.ModWheel, Model.ModWheelParameter);
        return true;
    }
}