// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IUnitData
    {
        private static partial ComResult unitDataSupported_ToManaged(IUnitData* self, UnitID unitID)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getUnitData_ToManaged(IUnitData* self, UnitID unitId, IBStream* data)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setUnitData_ToManaged(IUnitData* self, UnitID unitId, IBStream* data)
        {
            throw new NotImplementedException();
        }
    }
}