// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IParamValueQueue
    {
        private static partial LibVst.ParamID getParameterId_ToManaged(IParamValueQueue* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial int getPointCount_ToManaged(IParamValueQueue* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getPoint_ToManaged(IParamValueQueue* self, int index, int* sampleOffset, LibVst.ParamValue* value)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult addPoint_ToManaged(IParamValueQueue* self, int sampleOffset, LibVst.ParamValue value, int* index)
        {
            throw new NotImplementedException();
        }
    }
}
