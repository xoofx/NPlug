// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IPlugView
    {
        private static partial ComResult isPlatformTypeSupported_ccw(ComObject* self, FIDString type)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult attached_ccw(ComObject* self, void* parent, FIDString type)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult removed_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onWheel_ccw(ComObject* self, float distance)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onKeyDown_ccw(ComObject* self, char key, short keyCode, short modifiers)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onKeyUp_ccw(ComObject* self, char key, short keyCode, short modifiers)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getSize_ccw(ComObject* self, ViewRect* size)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onSize_ccw(ComObject* self, ViewRect* newSize)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult onFocus_ccw(ComObject* self, bool state)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setFrame_ccw(ComObject* self, IPlugFrame* frame)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult canResize_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult checkSizeConstraint_ccw(ComObject* self, ViewRect* rect)
        {
            throw new NotImplementedException();
        }
    }
}