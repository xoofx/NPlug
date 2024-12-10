// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IDataExchangeHandler
    {
        private static partial ComResult openQueue_ToManaged(IDataExchangeHandler* self, LibVst.IAudioProcessor* processor, uint blockSize, uint numBlocks, uint alignment, LibVst.DataExchangeUserContextID userContextID, LibVst.DataExchangeQueueID* outID)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult closeQueue_ToManaged(IDataExchangeHandler* self, LibVst.DataExchangeQueueID queueID)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult lockBlock_ToManaged(IDataExchangeHandler* self, LibVst.DataExchangeQueueID queueId, LibVst.DataExchangeBlock* block)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult freeBlock_ToManaged(IDataExchangeHandler* self, LibVst.DataExchangeQueueID queueId, LibVst.DataExchangeBlockID blockID, byte sendToController)
        {
            throw new NotImplementedException();
        }
    }
}
