// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IContextMenuTarget
    {
        private static partial ComResult executeMenuItem_ccw(ComObject* self, int tag)
        {
            throw new NotImplementedException();
        }
    }
}