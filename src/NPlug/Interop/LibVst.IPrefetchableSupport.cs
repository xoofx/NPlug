// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPrefetchableSupport
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioProcessorPrefetchable Get(IPrefetchableSupport* self) => ((ComObjectHandle*)self)->As<IAudioProcessorPrefetchable>();
        
        private static partial ComResult getPrefetchableSupport_ToManaged(IPrefetchableSupport* self, LibVst.PrefetchableSupport* prefetchable)
        {
            prefetchable->Value = (uint)Get(self).PrefetchableSupport;
            return true;
        }
    }
}
