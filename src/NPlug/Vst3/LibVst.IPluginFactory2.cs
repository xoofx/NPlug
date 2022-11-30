// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IPluginFactory2
    {
        private static partial ComResult getClassInfo2_ccw(IPluginFactory2* self, int index, PClassInfo2* info)
        {
            throw new NotImplementedException();
        }
    }
}