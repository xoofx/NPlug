// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IContextMenuTarget
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static AudioContextMenuAction Get(IContextMenuTarget* self) => ((ComObjectHandle*)self)->As<AudioContextMenuAction>();

        private static partial ComResult executeMenuItem_ToManaged(IContextMenuTarget* self, int tag)
        {
            Get(self)(tag);
            return true;
        }
    }
}