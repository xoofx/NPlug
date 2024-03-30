// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IStreamAttributes
    {
        private static partial ComResult getFileName_ToManaged(IStreamAttributes* self, LibVst.String128* name)
        {
            throw new NotImplementedException();
        }
        
        private static partial LibVst.IAttributeList* getAttributes_ToManaged(IStreamAttributes* self)
        {
            throw new NotImplementedException();
        }
    }
}
