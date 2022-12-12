// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IXmlRepresentationController
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerXmlRepresentation Get(IXmlRepresentationController* self) => (IAudioControllerXmlRepresentation)((ComObjectHandle*)self)->Target!;

        private static partial ComResult getXmlRepresentationStream_ToManaged(IXmlRepresentationController* self, LibVst.RepresentationInfo* info, LibVst.IBStream* stream)
        {
            var managed = Get(self);
            if (managed.Host is AudioHostApplicationClient hostClient)
            {
                Get(self).GetXmlRepresentationStream(new AudioControllerRepresentationInfo(
                    hostClient.GetOrCreateString(info->vendor, 64),
                    hostClient.GetOrCreateString(info->name, 64),
                    hostClient.GetOrCreateString(info->version, 64),
                    hostClient.GetOrCreateString(info->host, 64)
                ), IBStreamClient.GetStream(stream));
                return true;
            }

            return false;
        }
    }
}
