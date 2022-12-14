// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IAutomationState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerAutomationState Get(IAutomationState* self) => ((ComObjectHandle*)self)->As<IAudioControllerAutomationState>();

        private static partial ComResult setAutomationState_ToManaged(IAutomationState* self, int state)
        {
            Get(self).SetAutomationState((AudioControllerAutomationStates)state);
            return true;
        }
    }
}
