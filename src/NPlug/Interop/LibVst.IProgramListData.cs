// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IProgramListData
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioProcessorProgramListData Get(IProgramListData* self) => ((ComObjectHandle*)self)->As<IAudioProcessorProgramListData>();

        private static partial ComResult programDataSupported_ToManaged(IProgramListData* self, ProgramListID listId)
        {
            return Get(self).IsProgramDataSupported(new AudioProgramListId(listId.Value));
        }

        private static partial ComResult getProgramData_ToManaged(IProgramListData* self, ProgramListID listId, int programIndex, IBStream* data)
        {
            Get(self).GetProgramData(new AudioProgramListId(listId.Value), programIndex, IBStreamClient.GetStream(data));
            return true;
        }

        private static partial ComResult setProgramData_ToManaged(IProgramListData* self, ProgramListID listId, int programIndex, IBStream* data)
        {
            Get(self).SetProgramData(new AudioProgramListId(listId.Value), programIndex, IBStreamClient.GetStream(data));
            return true;
        }
    }
}