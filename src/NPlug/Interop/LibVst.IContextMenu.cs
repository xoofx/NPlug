// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IContextMenu
    {
        private static partial int getItemCount_ToManaged(IContextMenu* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getItem_ToManaged(IContextMenu* self, int index, LibVst.Item* item, LibVst.IContextMenuTarget** target)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult addItem_ToManaged(IContextMenu* self, LibVst.Item* item, LibVst.IContextMenuTarget* target)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult removeItem_ToManaged(IContextMenu* self, LibVst.Item* item, LibVst.IContextMenuTarget* target)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult popup_ToManaged(IContextMenu* self, LibVst.UCoord x, LibVst.UCoord y)
        {
            throw new NotImplementedException();
        }
    }
}
