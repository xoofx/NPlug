// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Vst3;


using System;

internal static unsafe partial class LibVst
{
    public partial struct ITestPlugProvider2
    {
        private static partial LibVst.IPluginFactory* getPluginFactory_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }
    }
}
