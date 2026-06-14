// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IMidiLearn2
    {
        private static partial ComResult onLiveMidi2ControllerInput_ToManaged(IMidiLearn2* self, LibVst.BusIndex index, LibVst.MidiChannel channel, LibVst.Midi2Controller midiCC)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult onLiveMidi1ControllerInput_ToManaged(IMidiLearn2* self, LibVst.BusIndex index, LibVst.MidiChannel channel, LibVst.CtrlNumber midiCC)
        {
            throw new NotImplementedException();
        }
    }
}
