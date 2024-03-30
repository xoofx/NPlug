// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IAttributes2
    {
        private static partial int countAttributes_ToManaged(IAttributes2* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial LibVst.IAttrID getAttributeID_ToManaged(IAttributes2* self, int index)
        {
            throw new NotImplementedException();
        }
    }
}
