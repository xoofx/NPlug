// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IComponentHandlerSystemTime
    {
        private static partial ComResult getSystemTime_ToManaged(IComponentHandlerSystemTime* self, long* systemTime)
        {
            throw new NotImplementedException();
        }
    }
}
