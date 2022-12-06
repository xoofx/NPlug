// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPlugViewContentScaleSupport
    {
        private static partial ComResult setContentScaleFactor_ccw(IPlugViewContentScaleSupport* self, LibVst.ScaleFactor factor)
        {
            throw new NotImplementedException();
        }
    }
}
