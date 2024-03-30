// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IProgress
    {
        private static partial ComResult start_ToManaged(IProgress* self, ProgressType type, char* optionalDescription, LibVst.ID* outID)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult update_ToManaged(IProgress* self, LibVst.ID id, LibVst.ParamValue normValue)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult finish_ToManaged(IProgress* self, LibVst.ID id)
        {
            throw new NotImplementedException();
        }
    }
}
