// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IAttributeList
    {
        private static partial ComResult setInt_ToManaged(IAttributeList* self, LibVst.AttrID id, long value)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getInt_ToManaged(IAttributeList* self, LibVst.AttrID id, long* value)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult setFloat_ToManaged(IAttributeList* self, LibVst.AttrID id, double value)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getFloat_ToManaged(IAttributeList* self, LibVst.AttrID id, double* value)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult setString_ToManaged(IAttributeList* self, LibVst.AttrID id, char* @string)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getString_ToManaged(IAttributeList* self, LibVst.AttrID id, char* @string, uint sizeInBytes)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult setBinary_ToManaged(IAttributeList* self, LibVst.AttrID id, void* data, uint sizeInBytes)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getBinary_ToManaged(IAttributeList* self, LibVst.AttrID id, void** data, uint* sizeInBytes)
        {
            throw new NotImplementedException();
        }
    }
}
