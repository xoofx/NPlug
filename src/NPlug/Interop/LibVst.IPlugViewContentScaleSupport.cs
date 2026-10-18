// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPlugViewContentScaleSupport
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioPluginView Get(IPlugViewContentScaleSupport* self) => ((ComObjectHandle*)self)->As<IAudioPluginView>();

        private static partial ComResult setContentScaleFactor_ToManaged(IPlugViewContentScaleSupport* self, LibVst.ScaleFactor factor)
        {
            // kResultFalse tells the host that the view does not scale itself.
            return Get(self).SetContentScaleFactor(factor.Value);
        }
    }
}
