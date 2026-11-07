// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct ITransportControl
    {
        private static partial ComResult requestAction_ToManaged(ITransportControl* self, int action, LibVst.TransportPosition* position)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult isActionSupported_ToManaged(ITransportControl* self, int action)
        {
            throw new NotImplementedException();
        }
    }
}
