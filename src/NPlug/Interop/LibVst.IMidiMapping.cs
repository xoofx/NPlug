// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IMidiMapping
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerMidiMapping Get(IMidiMapping* self) => ((ComObjectHandle*)self)->As<IAudioControllerMidiMapping>();

        private static partial ComResult getMidiControllerAssignment_ToManaged(IMidiMapping* self, int busIndex, short channel, LibVst.CtrlNumber midiControllerNumber, LibVst.ParamID* id)
        {
            return Get(self).TryGetMidiControllerAssignment(busIndex, channel, (AudioMidiControllerNumber)midiControllerNumber.Value, out *(AudioParameterId*)id);
        }
    }
}
