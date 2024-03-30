// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IComponentHandlerBusActivation
    {
        private static partial ComResult requestBusActivation_ToManaged(IComponentHandlerBusActivation* self, LibVst.MediaType type, LibVst.BusDirection dir, int index, byte state)
        {
            throw new NotImplementedException();
        }
    }
}
