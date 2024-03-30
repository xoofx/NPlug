// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IMessage
    {
        private static partial LibVst.FIDString getMessageID_ToManaged(IMessage* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial void setMessageID_ToManaged(IMessage* self, LibVst.FIDString id)
        {
            throw new NotImplementedException();
        }
        
        private static partial LibVst.IAttributeList* getAttributes_ToManaged(IMessage* self)
        {
            throw new NotImplementedException();
        }
    }
}
