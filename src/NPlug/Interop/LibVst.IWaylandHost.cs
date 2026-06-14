// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IWaylandHost
    {
        private static partial LibVst.wl_display* openWaylandConnection_ToManaged(IWaylandHost* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult closeWaylandConnection_ToManaged(IWaylandHost* self, LibVst.wl_display* display)
        {
            throw new NotImplementedException();
        }
    }
}
