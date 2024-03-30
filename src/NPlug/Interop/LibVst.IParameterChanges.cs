// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IParameterChanges
    {
        private static partial int getParameterCount_ToManaged(IParameterChanges* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial LibVst.IParamValueQueue* getParameterData_ToManaged(IParameterChanges* self, int index)
        {
            throw new NotImplementedException();
        }
        
        private static partial LibVst.IParamValueQueue* addParameterData_ToManaged(IParameterChanges* self, LibVst.ParamID* id, int* index)
        {
            throw new NotImplementedException();
        }
    }
}
