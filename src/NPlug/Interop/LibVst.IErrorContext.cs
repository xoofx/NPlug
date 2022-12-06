// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IErrorContext
    {
        private static partial void disableErrorUI_ccw(IErrorContext* self, byte state)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult errorMessageShown_ccw(IErrorContext* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getErrorMessage_ccw(IErrorContext* self, LibVst.IString* message)
        {
            throw new NotImplementedException();
        }
    }
}
