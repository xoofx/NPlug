// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IConnectionPoint
    {
        private static partial ComResult connect_ccw(ComObject* self, IConnectionPoint* other)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult disconnect_ccw(ComObject* self, IConnectionPoint* other)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult notify_ccw(ComObject* self, IMessage* message)
        {
            throw new NotImplementedException();
        }
    }
}