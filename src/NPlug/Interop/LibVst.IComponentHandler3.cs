// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IComponentHandler3
    {
        private static partial LibVst.IContextMenu* createContextMenu_ToManaged(IComponentHandler3* self, LibVst.IPlugView* plugView, LibVst.ParamID* paramID)
        {
            throw new NotImplementedException();
        }
    }
}
