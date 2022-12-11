// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPluginFactory2
    {
        private static IAudioPluginFactory Get(IPluginFactory2* self) => ((ComObjectHandle*)self)->As<IAudioPluginFactory>();

        private static partial ComResult getClassInfo2_ToManaged(IPluginFactory2* self, int index, PClassInfo2* info)
        {
            var pluginClassInfo = Get(self).GetPluginClassInfo(index);
            info->cid = pluginClassInfo.Id;
            info->cardinality = pluginClassInfo.Cardinality;
            //public fixed byte category[32];
            CopyStringToUTF8(pluginClassInfo is AudioProcessorClassInfo ? AudioEffectCategory : ComponentControllerCategory, info->category, 32);
            //public fixed byte name[64];
            CopyStringToUTF8(pluginClassInfo.Name, info->name, 64);
            info->classFlags = (uint)pluginClassInfo.ClassFlags;
            //public fixed byte subCategories[128];
            CopyStringToUTF8(pluginClassInfo is AudioProcessorClassInfo audioProcessorClassInfo ? GetPluginSubCategory(audioProcessorClassInfo.Category) : string.Empty, info->subCategories, 128);
            //public fixed byte vendor[64];
            CopyStringToUTF8(pluginClassInfo.Vendor, info->vendor, 64);
            var version = pluginClassInfo.Version.ToString();
            //public fixed byte version[64];
            CopyStringToUTF8(version, info->version, 64);
            //public fixed byte sdkVersion[64];
            CopyStringToUTF8(SdkVersion, info->sdkVersion, 64);
            return true;
        }
    }
}