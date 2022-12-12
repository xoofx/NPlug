// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IKeyswitchController
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerKeySwitch Get(IKeyswitchController* self) => (IAudioControllerKeySwitch)((ComObjectHandle*)self)->Target!;

        private static partial int getKeyswitchCount_ToManaged(IKeyswitchController* self, int busIndex, short channel)
        {
            return Get(self).GetKeySwitchCount(busIndex, channel);
        }

        private static partial ComResult getKeyswitchInfo_ToManaged(IKeyswitchController* self, int busIndex, short channel, int keySwitchIndex, LibVst.KeyswitchInfo* info)
        {
            var keySwitchInfo = Get(self).GetKeySwitchInfo(busIndex, channel, keySwitchIndex);
            info->typeId = new KeyswitchTypeID((uint)keySwitchInfo.TypeId);
            CopyStringToUTF16(keySwitchInfo.Title, ref info->title);
            CopyStringToUTF16(keySwitchInfo.ShortTitle, ref info->shortTitle);
            info->keyswitchMin = keySwitchInfo.KeySwitchMin;
            info->keyswitchMax = keySwitchInfo.KeySwitchMax;
            info->keyRemapped = keySwitchInfo.KeyRemapped;
            info->unitId = keySwitchInfo.UnitId.Value;
            info->flags = 0;
            return true;
        }
    }
}
