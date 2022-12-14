// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IEditController2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerExtended Get(IEditController2* self) => ((ComObjectHandle*)self)->As<IAudioControllerExtended>();

        private static partial ComResult setKnobMode_ToManaged(IEditController2* self, KnobMode mode)
        {
            return Get(self).TrySetKnobMode((AudioControllerKnobModes)mode.Value);
        }

        private static partial ComResult openHelp_ToManaged(IEditController2* self, byte onlyCheck)
        {
            return Get(self).TryOpenHelp(onlyCheck != 0);
        }

        private static partial ComResult openAboutBox_ToManaged(IEditController2* self, byte onlyCheck)
        {
            return Get(self).TryOpenAboutBox(onlyCheck != 0);
        }
    }
}