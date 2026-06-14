// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IMidiMapping2
    {
        private static partial uint getNumMidi2ControllerAssignments_ToManaged(IMidiMapping2* self, BusDirections direction)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getMidi2ControllerAssignments_ToManaged(IMidiMapping2* self, BusDirections direction, LibVst.Midi2ControllerParamIDAssignmentList* list)
        {
            throw new NotImplementedException();
        }
        
        private static partial uint getNumMidi1ControllerAssignments_ToManaged(IMidiMapping2* self, BusDirections direction)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getMidi1ControllerAssignments_ToManaged(IMidiMapping2* self, BusDirections direction, LibVst.Midi1ControllerParamIDAssignmentList* list)
        {
            throw new NotImplementedException();
        }
    }
}
