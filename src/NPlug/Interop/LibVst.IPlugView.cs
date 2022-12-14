// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPlugView
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioPluginView Get(IPlugView* self) => ((ComObjectHandle*)self)->As<IAudioPluginView>();

        private static partial ComResult isPlatformTypeSupported_ToManaged(IPlugView* self, FIDString type)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult attached_ToManaged(IPlugView* self, void* parent, FIDString type)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult removed_ToManaged(IPlugView* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onWheel_ToManaged(IPlugView* self, float distance)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onKeyDown_ToManaged(IPlugView* self, ushort key, short keyCode, short modifiers)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onKeyUp_ToManaged(IPlugView* self, ushort key, short keyCode, short modifiers)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getSize_ToManaged(IPlugView* self, ViewRect* size)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onSize_ToManaged(IPlugView* self, ViewRect* newSize)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onFocus_ToManaged(IPlugView* self, byte state)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setFrame_ToManaged(IPlugView* self, IPlugFrame* frame)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult canResize_ToManaged(IPlugView* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult checkSizeConstraint_ToManaged(IPlugView* self, ViewRect* rect)
        {
            throw new NotImplementedException();
        }
    }
}