// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Vst3;


using System;

internal static unsafe partial class LibVst
{
    public partial struct ITestPlugProvider
    {
        private static partial LibVst.IComponent* getComponent_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial LibVst.IEditController* getController_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult releasePlugIn_ccw(ComObject* self, LibVst.IComponent* component, LibVst.IEditController* controller)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getSubCategories_ccw(ComObject* self, LibVst.IStringResult* result)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getComponentUID_ccw(ComObject* self, LibVst.FUID* uid)
        {
            throw new NotImplementedException();
        }
    }
}
