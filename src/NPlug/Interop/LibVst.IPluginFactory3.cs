// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPluginFactory3
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioPluginFactory Get(IPluginFactory3* self) => ((ComObjectHandle*)self)->As<IAudioPluginFactory>();

        private static partial ComResult getClassInfoUnicode_ToManaged(IPluginFactory3* self, int index, PClassInfoW* info)
        {
            var pluginClassInfo = Get(self).GetPluginClassInfo(index);
            info->cid = pluginClassInfo.ClassId.ConvertToPlatform();
            info->cardinality = pluginClassInfo.Cardinality;
            //public fixed byte category[32];
            CopyStringToUTF8(GetPluginCategory(pluginClassInfo), info->category, 32);
            //public fixed char name[64];
            CopyStringToUTF16(pluginClassInfo.Name, info->name, 64);
            info->classFlags = (uint)pluginClassInfo.ClassFlags;
            //public fixed byte subCategories[128];
            CopyStringToUTF8(pluginClassInfo is AudioProcessorClassInfo audioProcessorClassInfo ? GetPluginSubCategory(audioProcessorClassInfo.Category) : string.Empty, info->subCategories, 128);
            //public fixed char vendor[64];
            CopyStringToUTF16(pluginClassInfo.Vendor, info->vendor, 64);
            var version = pluginClassInfo.Version.ToString();
            //public fixed char version[64];
            CopyStringToUTF16(version, info->version, 64);
            //public fixed byte sdkVersion[64];
            CopyStringToUTF16(SdkVersion, info->sdkVersion, 64);
            return true;
        }

        private static partial ComResult setHostContext_ToManaged(IPluginFactory3* self, FUnknown* context)
        {
            // Not implemented in the SDK, so not sure what it can be used for...
            return ComResult.NotImplemented;
        }
    }
}