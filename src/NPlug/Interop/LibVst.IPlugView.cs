// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPlugView
    {
        private static partial ComResult isPlatformTypeSupported_ccw(IPlugView* self, FIDString type)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult attached_ccw(IPlugView* self, void* parent, FIDString type)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult removed_ccw(IPlugView* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onWheel_ccw(IPlugView* self, float distance)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onKeyDown_ccw(IPlugView* self, ushort key, short keyCode, short modifiers)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onKeyUp_ccw(IPlugView* self, ushort key, short keyCode, short modifiers)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getSize_ccw(IPlugView* self, ViewRect* size)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onSize_ccw(IPlugView* self, ViewRect* newSize)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onFocus_ccw(IPlugView* self, byte state)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setFrame_ccw(IPlugView* self, IPlugFrame* frame)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult canResize_ccw(IPlugView* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult checkSizeConstraint_ccw(IPlugView* self, ViewRect* rect)
        {
            throw new NotImplementedException();
        }
    }
}