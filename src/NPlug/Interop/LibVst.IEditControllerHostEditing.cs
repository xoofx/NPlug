// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IEditControllerHostEditing
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerHostEditing Get(IEditControllerHostEditing* self) => (IAudioControllerHostEditing)((ComObjectHandle*)self)->Target!;

        private static partial ComResult beginEditFromHost_ToManaged(IEditControllerHostEditing* self, LibVst.ParamID paramID)
        {
            Get(self).BeginEditFromHost(paramID);
            return true;
        }

        private static partial ComResult endEditFromHost_ToManaged(IEditControllerHostEditing* self, LibVst.ParamID paramID)
        {
            Get(self).EndEditFromHost(paramID);
            return true;
        }
    }
}
