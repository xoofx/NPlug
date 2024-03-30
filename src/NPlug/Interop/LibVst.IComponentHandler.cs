// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IComponentHandler
    {
        private static partial ComResult beginEdit_ToManaged(IComponentHandler* self, LibVst.ParamID id)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult performEdit_ToManaged(IComponentHandler* self, LibVst.ParamID id, LibVst.ParamValue valueNormalized)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult endEdit_ToManaged(IComponentHandler* self, LibVst.ParamID id)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult restartComponent_ToManaged(IComponentHandler* self, int flags)
        {
            throw new NotImplementedException();
        }
    }
}
