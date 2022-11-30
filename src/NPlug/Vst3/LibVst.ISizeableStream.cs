// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Vst3;


using System;

internal static unsafe partial class LibVst
{
    public partial struct ISizeableStream
    {
        private static partial ComResult getStreamSize_ccw(ISizeableStream* self, long* size)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult setStreamSize_ccw(ISizeableStream* self, long size)
        {
            throw new NotImplementedException();
        }
    }
}
