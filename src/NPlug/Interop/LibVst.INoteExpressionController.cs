// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct INoteExpressionController
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerNoteExpression Get(INoteExpressionController* self) => ((ComObjectHandle*)self)->As<IAudioControllerNoteExpression>();

        private static partial int getNoteExpressionCount_ToManaged(INoteExpressionController* self, int busIndex, short channel)
        {
            return Get(self).GetNoteExpressionCount(busIndex, channel);
        }

        private static partial ComResult getNoteExpressionInfo_ToManaged(INoteExpressionController* self, int busIndex, short channel, int noteExpressionIndex, LibVst.NoteExpressionTypeInfo* info)
        {
            var managedInfo = Get(self).GetNoteExpressionInfo(busIndex, channel, noteExpressionIndex);
            info->typeId = new((uint)managedInfo.TypeId);
            CopyStringToUTF16(managedInfo.Title, ref info->title);
            CopyStringToUTF16(managedInfo.ShortTitle, ref info->shortTitle);
            CopyStringToUTF16(managedInfo.Units, ref info->units);
            info->unitId = managedInfo.UnitId.Value;
            Debug.Assert(sizeof(LibVst.NoteExpressionValueDescription) == sizeof(AudioNoteExpressionValueDescription));
            var localDesc = managedInfo.ValueDescription;
            info->valueDesc = Unsafe.As<AudioNoteExpressionValueDescription, NoteExpressionValueDescription>(ref localDesc);
            info->associatedParameterId = managedInfo.AssociatedParameterId;
            info->flags = (int)managedInfo.Flags;
            return true;
        }

        private static partial ComResult getNoteExpressionStringByValue_ToManaged(INoteExpressionController* self, int busIndex, short channel, LibVst.NoteExpressionTypeID id, LibVst.NoteExpressionValue valueNormalized, LibVst.String128* @string)
        {
            var text = Get(self).GetNoteExpressionStringByValue(busIndex, channel, (AudioNoteExpressionTypeId)id.Value, valueNormalized.Value);
            CopyStringToUTF16(text, ref *@string);
            return true;
        }

        private static partial ComResult getNoteExpressionValueByString_ToManaged(INoteExpressionController* self, int busIndex, short channel, LibVst.NoteExpressionTypeID id, char* @string, LibVst.NoteExpressionValue* valueNormalized)
        {
            var text = ((String128*)@string)->ToString();
            *((double*)valueNormalized) = Get(self).GetNoteExpressionValueByString(busIndex, channel, (AudioNoteExpressionTypeId)id.Value, text);
            return true;
        }
    }
}
