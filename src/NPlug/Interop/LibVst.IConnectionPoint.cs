// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IConnectionPoint
    {
        private static partial ComResult connect_ToManaged(IConnectionPoint* self, IConnectionPoint* other)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult disconnect_ToManaged(IConnectionPoint* self, IConnectionPoint* other)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult notify_ToManaged(IConnectionPoint* self, IMessage* message)
        {
            throw new NotImplementedException();
        }
    }
}