// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug.SimpleProgramChange;

public class SimpleProgramChangeController : AudioController<SimpleProgramChangeModel>
{
    public static readonly Guid ClassId = new("59c17896-478b-4af1-86d8-3ba0817149d5");

    protected override bool Initialize(AudioHostApplication host)
    {
        SetMappingBusToUnit(BusMediaType.Event, BusDirection.Input, 0, 0, Model);
        return true;
    }
}
