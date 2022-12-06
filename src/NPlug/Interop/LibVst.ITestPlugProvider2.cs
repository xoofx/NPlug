// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct ITestPlugProvider2
    {
        private static partial LibVst.IPluginFactory* getPluginFactory_ccw(ITestPlugProvider2* self)
        {
            throw new NotImplementedException();
        }
    }
}
