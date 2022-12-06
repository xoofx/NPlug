// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPluginFactory3
    {
        private static partial ComResult getClassInfoUnicode_ccw(IPluginFactory3* self, int index, PClassInfoW* info)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setHostContext_ccw(IPluginFactory3* self, FUnknown* context)
        {
            throw new NotImplementedException();
        }
    }
}