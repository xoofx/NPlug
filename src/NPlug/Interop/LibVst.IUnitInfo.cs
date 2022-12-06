// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IUnitInfo
    {
        private static partial int getUnitCount_ccw(IUnitInfo* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getUnitInfo_ccw(IUnitInfo* self, int unitIndex, UnitInfo* info)
        {
            throw new NotImplementedException();
        }

        private static partial int getProgramListCount_ccw(IUnitInfo* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getProgramListInfo_ccw(IUnitInfo* self, int listIndex, ProgramListInfo* info)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getProgramName_ccw(IUnitInfo* self, ProgramListID listId, int programIndex, String128* name)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getProgramInfo_ccw(IUnitInfo* self, ProgramListID listId, int programIndex, CString attributeId, String128* attributeValue)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult hasProgramPitchNames_ccw(IUnitInfo* self, ProgramListID listId, int programIndex)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getProgramPitchName_ccw(IUnitInfo* self, LibVst.ProgramListID listId, int programIndex, short midiPitch, LibVst.String128* name)
        {
            throw new NotImplementedException();
        }

        private static partial UnitID getSelectedUnit_ccw(IUnitInfo* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult selectUnit_ccw(IUnitInfo* self, UnitID unitId)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getUnitByBus_ccw(IUnitInfo* self, MediaType type, BusDirection dir, int busIndex, int channel, UnitID* unitId)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setUnitProgramData_ccw(IUnitInfo* self, int listOrUnitId, int programIndex, IBStream* data)
        {
            throw new NotImplementedException();
        }
    }
}