// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace NPlug.Vst3;

internal static partial class LibVst
{
    public unsafe struct ComObject
    {
        public void** Vtbl;
        public uint RefCount;
        public SpinLock Lock;
        public GCHandle Handle;
    }

    public unsafe interface INativeGuid
    {
        public static abstract Guid* NativeGuid { get; }
    }
}