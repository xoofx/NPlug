// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IMidiLearn
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerMidiLearn Get(IMidiLearn* self) => ((ComObjectHandle*)self)->As<IAudioControllerMidiLearn>();

        
        private static partial ComResult onLiveMIDIControllerInput_ToManaged(IMidiLearn* self, int busIndex, short channel, LibVst.CtrlNumber midiCC)
        {
            return Get(self).TryOnLiveMidiControllerInput(busIndex, channel, (AudioMidiControllerNumber)midiCC.Value);
        }
    }
}
