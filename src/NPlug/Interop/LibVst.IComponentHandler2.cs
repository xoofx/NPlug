// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IComponentHandler2
    {
        private static partial ComResult setDirty_ToManaged(IComponentHandler2* self, byte state)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult requestOpenEditor_ToManaged(IComponentHandler2* self, LibVst.FIDString name)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult startGroupEdit_ToManaged(IComponentHandler2* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult finishGroupEdit_ToManaged(IComponentHandler2* self)
        {
            throw new NotImplementedException();
        }
    }
}
