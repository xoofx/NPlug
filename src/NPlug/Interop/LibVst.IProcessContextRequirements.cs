// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IProcessContextRequirements
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NPlug.IAudioProcessor Get(IProcessContextRequirements* self) => (NPlug.IAudioProcessor)((ComObjectHandle*)self)->Target!;

        private static partial uint getProcessContextRequirements_ToManaged(IProcessContextRequirements* self)
        {
            return (uint)Get(self).ProcessContextRequirementFlags;
        }
    }
}