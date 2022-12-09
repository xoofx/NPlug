// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IProgramListData
    {
        private static partial ComResult programDataSupported_ToManaged(IProgramListData* self, ProgramListID listId)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getProgramData_ToManaged(IProgramListData* self, ProgramListID listId, int programIndex, IBStream* data)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setProgramData_ToManaged(IProgramListData* self, ProgramListID listId, int programIndex, IBStream* data)
        {
            throw new NotImplementedException();
        }
    }
}