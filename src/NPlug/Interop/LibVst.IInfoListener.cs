// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IInfoListener
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerInfoListener Get(IInfoListener* self) => ((ComObjectHandle*)self)->As<IAudioControllerInfoListener>();

        private static partial ComResult setChannelContextInfos_ToManaged(IInfoListener* self, LibVst.IAttributeList* list)
        {
            var controller = Get(self);
            // Should be always the case, but in case
            if (controller.Host is AudioHostApplicationClient hostClient)
            {
                Get(self).SetChannelContextInfos(new AudioAttributeList(hostClient, (nint)list));
                return true;
            }

            return false;
        }
    }
}
