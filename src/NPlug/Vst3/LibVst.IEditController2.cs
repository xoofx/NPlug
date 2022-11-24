// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IEditController2
    {
        private static partial ComResult setKnobMode_ccw(ComObject* self, KnobMode mode)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult openHelp_ccw(ComObject* self, bool onlyCheck)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult openAboutBox_ccw(ComObject* self, bool onlyCheck)
        {
            throw new NotImplementedException();
        }
    }
}