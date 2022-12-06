// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct ITestPlugProvider
    {
        private static partial LibVst.IComponent* getComponent_ccw(ITestPlugProvider* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial LibVst.IEditController* getController_ccw(ITestPlugProvider* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult releasePlugIn_ccw(ITestPlugProvider* self, LibVst.IComponent* component, LibVst.IEditController* controller)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getSubCategories_ccw(ITestPlugProvider* self, LibVst.IStringResult* result)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getComponentUID_ccw(ITestPlugProvider* self, LibVst.FUID* uid)
        {
            throw new NotImplementedException();
        }
    }
}
