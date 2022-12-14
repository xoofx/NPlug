// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct INoteExpressionPhysicalUIMapping
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerNoteExpressionPhysicalUIMapping Get(INoteExpressionPhysicalUIMapping* self) => ((ComObjectHandle*)self)->As<IAudioControllerNoteExpressionPhysicalUIMapping>();
        
        private static partial ComResult getPhysicalUIMapping_ToManaged(INoteExpressionPhysicalUIMapping* self, int busIndex, short channel, LibVst.PhysicalUIMapList* list)
        {
            return Get(self).TryGetPhysicalUIMapping(busIndex, channel, new Span<AudioPhysicalUIMap>(list->map, (int)list->count));
        }
    }
}
