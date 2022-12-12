// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IUnitData
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioProcessorUnitData Get(IUnitData* self) => (IAudioProcessorUnitData)((ComObjectHandle*)self)->Target!;

        private static partial ComResult unitDataSupported_ToManaged(IUnitData* self, UnitID unitID)
        {
            return Get(self).IsUnitDataSupported(new AudioUnitId(unitID.Value));
        }

        private static partial ComResult getUnitData_ToManaged(IUnitData* self, UnitID unitId, IBStream* data)
        {
            Get(self).GetUnitData(new AudioUnitId(unitId.Value), IBStreamClient.GetStream(data));
            return true;
        }

        private static partial ComResult setUnitData_ToManaged(IUnitData* self, UnitID unitId, IBStream* data)
        {
            Get(self).SetUnitData(new AudioUnitId(unitId.Value), IBStreamClient.GetStream(data));
            return true;
        }
    }
}