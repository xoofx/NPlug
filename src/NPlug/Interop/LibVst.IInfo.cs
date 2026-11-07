// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    /// <summary>
    /// Vst::NoteOnOrchestralArticulation::IInfo
    /// </summary>
    public partial struct IInfo
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerNoteOnOrchestralArticulationInfo Get(IInfo* self) => ((ComObjectHandle*)self)->As<IAudioControllerNoteOnOrchestralArticulationInfo>();

        private static partial ComResult getVariationsInfo_ToManaged(IInfo* self, int busIndex, short channel, LibVst.ClassificationVariations* info)
        {
            if (info is null)
            {
                return ComResult.InvalidArg;
            }

            // Same layout: 17 classifications of 16 bytes.
            ref var variations = ref Unsafe.AsRef<AudioOrchestralArticulationVariations>(info);
            variations.Clear();
            return Get(self).TryGetOrchestralArticulationVariations(busIndex, channel, ref variations);
        }
    }
}
