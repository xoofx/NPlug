// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Vst3;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IPersistent
    {
        private static partial ComResult getClassID_ccw(ComObject* self, byte* uid)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult saveAttributes_ccw(ComObject* self, LibVst.IAttributes* arg)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult loadAttributes_ccw(ComObject* self, LibVst.IAttributes* arg)
        {
            throw new NotImplementedException();
        }
    }
}
