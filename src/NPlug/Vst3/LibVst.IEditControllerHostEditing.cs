// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Vst3;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IEditControllerHostEditing
    {
        private static partial ComResult beginEditFromHost_ccw(IEditControllerHostEditing* self, LibVst.ParamID paramID)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult endEditFromHost_ccw(IEditControllerHostEditing* self, LibVst.ParamID paramID)
        {
            throw new NotImplementedException();
        }
    }
}
