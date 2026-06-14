// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IWaylandFrame
    {
        private static partial LibVst.wl_surface* getWaylandSurface_ToManaged(IWaylandFrame* self, LibVst.wl_display* display)
        {
            throw new NotImplementedException();
        }
        
        private static partial LibVst.xdg_surface* getParentSurface_ToManaged(IWaylandFrame* self, LibVst.ViewRect* parentSize, LibVst.wl_display* display)
        {
            throw new NotImplementedException();
        }
        
        private static partial LibVst.xdg_toplevel* getParentToplevel_ToManaged(IWaylandFrame* self, LibVst.wl_display* display)
        {
            throw new NotImplementedException();
        }
    }
}
