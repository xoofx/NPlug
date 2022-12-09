// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPersistent
    {
        private static partial ComResult getClassID_ToManaged(IPersistent* self, byte* uid)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult saveAttributes_ToManaged(IPersistent* self, LibVst.IAttributes* arg)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult loadAttributes_ToManaged(IPersistent* self, LibVst.IAttributes* arg)
        {
            throw new NotImplementedException();
        }
    }
}
