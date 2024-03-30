// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IPlugFrame
    {
        private static partial ComResult resizeView_ToManaged(IPlugFrame* self, LibVst.IPlugView* view, LibVst.ViewRect* newSize)
        {
            throw new NotImplementedException();
        }
    }
}
