// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IHostApplication
    {
        private static partial ComResult getName_ToManaged(IHostApplication* self, LibVst.String128* name)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult createInstance_ToManaged(IHostApplication* self, Guid* cid, Guid* _iid, void** obj)
        {
            throw new NotImplementedException();
        }
    }
}
