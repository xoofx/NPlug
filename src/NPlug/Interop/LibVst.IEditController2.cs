// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IEditController2
    {
        private static partial ComResult setKnobMode_ccw(IEditController2* self, KnobMode mode)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult openHelp_ccw(IEditController2* self, byte onlyCheck)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult openAboutBox_ccw(IEditController2* self, byte onlyCheck)
        {
            throw new NotImplementedException();
        }
    }
}