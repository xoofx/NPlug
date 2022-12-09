// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IXmlRepresentationController
    {
        private static partial ComResult getXmlRepresentationStream_ToManaged(IXmlRepresentationController* self, LibVst.RepresentationInfo* info, LibVst.IBStream* stream)
        {
            throw new NotImplementedException();
        }
    }
}
