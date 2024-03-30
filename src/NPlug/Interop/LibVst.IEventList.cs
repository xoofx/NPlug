// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IEventList
    {
        private static partial int getEventCount_ToManaged(IEventList* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getEvent_ToManaged(IEventList* self, int index, LibVst.Event* e)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult addEvent_ToManaged(IEventList* self, LibVst.Event* e)
        {
            throw new NotImplementedException();
        }
    }
}
