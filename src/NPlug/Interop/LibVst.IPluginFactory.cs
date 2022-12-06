// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPluginFactory
    {
        private static partial ComResult getFactoryInfo_ccw(IPluginFactory* self, PFactoryInfo* info)
        {
            throw new NotImplementedException();
        }

        private static partial int countClasses_ccw(IPluginFactory* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getClassInfo_ccw(IPluginFactory* self, int index, PClassInfo* info)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult createInstance_ccw(IPluginFactory* self, FIDString cid, FIDString _iid, void** obj)
        {
            throw new NotImplementedException();
        }
    }
}