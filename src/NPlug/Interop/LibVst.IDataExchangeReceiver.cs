// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IDataExchangeReceiver
    {
        private static partial void queueOpened_ToManaged(IDataExchangeReceiver* self, LibVst.DataExchangeUserContextID userContextID, uint blockSize, byte* dispatchOnBackgroundThread)
        {
            throw new NotImplementedException();
        }
        
        private static partial void queueClosed_ToManaged(IDataExchangeReceiver* self, LibVst.DataExchangeUserContextID userContextID)
        {
            throw new NotImplementedException();
        }
        
        private static partial void onDataExchangeBlocksReceived_ToManaged(IDataExchangeReceiver* self, LibVst.DataExchangeUserContextID userContextID, uint numBlocks, LibVst.DataExchangeBlock* blocks, byte onBackgroundThread)
        {
            throw new NotImplementedException();
        }
    }
}
