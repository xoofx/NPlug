// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IEventHandler
    {
        private static partial void onFDIsSet_ToManaged(IEventHandler* self, LibVst.FileDescriptor fd)
        {
            throw new NotImplementedException();
        }
    }
}
