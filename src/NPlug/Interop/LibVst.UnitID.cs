// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial record struct UnitID
    {
        public static implicit operator AudioUnitId(UnitID id) => new(id.Value);
        public static implicit operator UnitID(AudioUnitId id) => new(id.Value);
    }
}
