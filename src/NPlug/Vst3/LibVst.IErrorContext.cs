// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Vst3;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IErrorContext
    {
        private static partial void disableErrorUI_ccw(ComObject* self, bool state)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult errorMessageShown_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getErrorMessage_ccw(ComObject* self, LibVst.IString* message)
        {
            throw new NotImplementedException();
        }
    }
}
