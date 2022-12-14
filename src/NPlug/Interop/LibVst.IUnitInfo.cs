// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IUnitInfo
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerUnitInfo Get(IUnitInfo* self) => ((ComObjectHandle*)self)->As<IAudioControllerUnitInfo>();

        private static partial int getUnitCount_ToManaged(IUnitInfo* self)
        {
            return Get(self).UnitCount;
        }

        private static partial ComResult getUnitInfo_ToManaged(IUnitInfo* self, int unitIndex, UnitInfo* info)
        {
            var managedInfo = Get(self).GetUnitInfo(unitIndex);
            info->id = new UnitID(managedInfo.Id.Value);
            info->parentUnitId = new UnitID(managedInfo.ParentUnitId.Value);
            info->name.CopyFrom(managedInfo.Name);
            info->programListId = new ProgramListID(managedInfo.ProgramListId.Value);
            return true;
        }

        private static partial int getProgramListCount_ToManaged(IUnitInfo* self)
        {
            return Get(self).ProgramListCount;
        }

        private static partial ComResult getProgramListInfo_ToManaged(IUnitInfo* self, int listIndex, ProgramListInfo* info)
        {
            var managedInfo = Get(self).GetProgramListInfo(listIndex);
            info->id = new ProgramListID(managedInfo.Id.Value);
            info->name.CopyFrom(managedInfo.Name);
            info->programCount = managedInfo.ProgramCount;
            return true;
        }

        private static partial ComResult getProgramName_ToManaged(IUnitInfo* self, ProgramListID listId, int programIndex, String128* name)
        {
            name->CopyFrom(Get(self).GetProgramName(new AudioProgramListId(listId.Value), programIndex));
            return true;
        }

        private static partial ComResult getProgramInfo_ToManaged(IUnitInfo* self, ProgramListID listId, int programIndex, CString attributeId, String128* attributeValue)
        {
            var managedUnit = Get(self);
            if (managedUnit.Host is AudioHostApplicationClient client)
            {
                attributeValue->CopyFrom(Get(self).GetProgramInfo(new AudioProgramListId(listId.Value), programIndex, client.GetOrCreateString(attributeId.Value)));
                return true;
            }

            return false;
        }

        private static partial ComResult hasProgramPitchNames_ToManaged(IUnitInfo* self, ProgramListID listId, int programIndex)
        {
            return Get(self).HasProgramPitchNames(new AudioProgramListId(listId.Value), programIndex);
        }

        private static partial ComResult getProgramPitchName_ToManaged(IUnitInfo* self, LibVst.ProgramListID listId, int programIndex, short midiPitch, LibVst.String128* name)
        {
            name->CopyFrom(Get(self).GetProgramPitchName(new AudioProgramListId(listId.Value), programIndex, midiPitch));
            return true;
        }

        private static partial UnitID getSelectedUnit_ToManaged(IUnitInfo* self)
        {
            return new UnitID(Get(self).SelectedUnit.Value);
        }

        private static partial ComResult selectUnit_ToManaged(IUnitInfo* self, UnitID unitId)
        {
            Get(self).SelectedUnit = new AudioUnitId(unitId.Value);
            return true;
        }

        private static partial ComResult getUnitByBus_ToManaged(IUnitInfo* self, MediaType type, BusDirection dir, int busIndex, int channel, UnitID* unitId)
        {
            Get(self).GetUnitByBus((BusMediaType)type.Value, (NPlug.BusDirection)dir.Value, busIndex, channel, out *(AudioUnitId*)unitId);
            return true;
        }

        private static partial ComResult setUnitProgramData_ToManaged(IUnitInfo* self, int listOrUnitId, int programIndex, IBStream* data)
        {
            Get(self).SetUnitProgramData(listOrUnitId, programIndex, IBStreamClient.GetStream(data));
            return true;
        }
    }
}