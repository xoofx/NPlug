// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct INoteExpressionController
    {
        private static partial int getNoteExpressionCount_ToManaged(INoteExpressionController* self, int busIndex, short channel)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getNoteExpressionInfo_ToManaged(INoteExpressionController* self, int busIndex, short channel, int noteExpressionIndex, LibVst.NoteExpressionTypeInfo* info)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getNoteExpressionStringByValue_ToManaged(INoteExpressionController* self, int busIndex, short channel, LibVst.NoteExpressionTypeID id, LibVst.NoteExpressionValue valueNormalized, LibVst.String128* @string)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getNoteExpressionValueByString_ToManaged(INoteExpressionController* self, int busIndex, short channel, LibVst.NoteExpressionTypeID id, char* @string, LibVst.NoteExpressionValue* valueNormalized)
        {
            throw new NotImplementedException();
        }
    }
}
