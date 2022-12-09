// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IUnitInfo
    {
        private static partial int getUnitCount_ToManaged(IUnitInfo* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getUnitInfo_ToManaged(IUnitInfo* self, int unitIndex, UnitInfo* info)
        {
            throw new NotImplementedException();
        }

        private static partial int getProgramListCount_ToManaged(IUnitInfo* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getProgramListInfo_ToManaged(IUnitInfo* self, int listIndex, ProgramListInfo* info)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getProgramName_ToManaged(IUnitInfo* self, ProgramListID listId, int programIndex, String128* name)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getProgramInfo_ToManaged(IUnitInfo* self, ProgramListID listId, int programIndex, CString attributeId, String128* attributeValue)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult hasProgramPitchNames_ToManaged(IUnitInfo* self, ProgramListID listId, int programIndex)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getProgramPitchName_ToManaged(IUnitInfo* self, LibVst.ProgramListID listId, int programIndex, short midiPitch, LibVst.String128* name)
        {
            throw new NotImplementedException();
        }

        private static partial UnitID getSelectedUnit_ToManaged(IUnitInfo* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult selectUnit_ToManaged(IUnitInfo* self, UnitID unitId)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getUnitByBus_ToManaged(IUnitInfo* self, MediaType type, BusDirection dir, int busIndex, int channel, UnitID* unitId)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setUnitProgramData_ToManaged(IUnitInfo* self, int listOrUnitId, int programIndex, IBStream* data)
        {
            throw new NotImplementedException();
        }
    }
}