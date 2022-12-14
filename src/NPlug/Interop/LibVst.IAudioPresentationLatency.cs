// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IAudioPresentationLatency
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NPlug.IAudioProcessor Get(IAudioPresentationLatency* self) => ((ComObjectHandle*)self)->As<NPlug.IAudioProcessor>();


        private static partial ComResult setAudioPresentationLatencySamples_ToManaged(IAudioPresentationLatency* self, LibVst.BusDirection dir, int busIndex, uint latencyInSamples)
        {
            Get(self).SetAudioPresentationLatencySamples((NPlug.BusDirection)dir.Value, busIndex, latencyInSamples);
            return true;
        }
    }
}
