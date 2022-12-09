// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IDependent
    {
        private static partial void update_ToManaged(IDependent* self, LibVst.FUnknown* changedUnknown, int message)
        {
            throw new NotImplementedException();
        }
    }
}
