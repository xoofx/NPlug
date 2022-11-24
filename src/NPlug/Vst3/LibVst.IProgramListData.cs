// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IProgramListData
    {
        private static partial ComResult programDataSupported_ccw(ComObject* self, ProgramListID listId)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getProgramData_ccw(ComObject* self, ProgramListID listId, int programIndex, IBStream* data)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setProgramData_ccw(ComObject* self, ProgramListID listId, int programIndex, IBStream* data)
        {
            throw new NotImplementedException();
        }
    }
}