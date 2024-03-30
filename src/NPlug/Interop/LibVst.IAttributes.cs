// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IAttributes
    {
        private static partial ComResult set_ToManaged(IAttributes* self, byte* attrID, LibVst.FVariant* data)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult queue_ToManaged(IAttributes* self, byte* listID, LibVst.FVariant* data)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult setBinaryData_ToManaged(IAttributes* self, byte* attrID, void* data, uint bytes, byte copyBytes)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult get_ToManaged(IAttributes* self, byte* attrID, LibVst.FVariant* data)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult unqueue_ToManaged(IAttributes* self, byte* listID, LibVst.FVariant* data)
        {
            throw new NotImplementedException();
        }
        
        private static partial int getQueueItemCount_ToManaged(IAttributes* self, byte* arg)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult resetQueue_ToManaged(IAttributes* self, byte* attrID)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult resetAllQueues_ToManaged(IAttributes* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getBinaryData_ToManaged(IAttributes* self, byte* attrID, void* data, uint bytes)
        {
            throw new NotImplementedException();
        }
        
        private static partial uint getBinaryDataSize_ToManaged(IAttributes* self, byte* attrID)
        {
            throw new NotImplementedException();
        }
    }
}
