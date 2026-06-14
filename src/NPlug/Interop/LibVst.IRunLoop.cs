// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IRunLoop
    {
        private static partial ComResult registerEventHandler_ToManaged(IRunLoop* self, LibVst.IEventHandler* handler, LibVst.FileDescriptor fd)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult unregisterEventHandler_ToManaged(IRunLoop* self, LibVst.IEventHandler* handler)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult registerTimer_ToManaged(IRunLoop* self, LibVst.ITimerHandler* handler, LibVst.TimerInterval milliseconds)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult unregisterTimer_ToManaged(IRunLoop* self, LibVst.ITimerHandler* handler)
        {
            throw new NotImplementedException();
        }
    }
}
