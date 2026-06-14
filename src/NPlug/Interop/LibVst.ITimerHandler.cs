// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct ITimerHandler
    {
        private static partial void onTimer_ToManaged(ITimerHandler* self)
        {
            throw new NotImplementedException();
        }
    }
}
