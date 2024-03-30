// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IUnitHandler
    {
        private static partial ComResult notifyUnitSelection_ToManaged(IUnitHandler* self, LibVst.UnitID unitId)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult notifyProgramListChange_ToManaged(IUnitHandler* self, LibVst.ProgramListID listId, int programIndex)
        {
            throw new NotImplementedException();
        }
    }
}
