// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Vst3;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IString
    {
        private static partial void setText8_ccw(ComObject* self, byte* text)
        {
            throw new NotImplementedException();
        }
        
        private static partial void setText16_ccw(ComObject* self, char* text)
        {
            throw new NotImplementedException();
        }
        
        private static partial byte* getText8_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial char* getText16_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial void take_ccw(ComObject* self, void* s, byte isWide)
        {
            throw new NotImplementedException();
        }
        
        private static partial byte isWideString_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }
    }
}
