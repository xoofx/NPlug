// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Reflection;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    private static string GetPluginCategory(AudioPluginClassInfo classInfo) => GetPluginCategory(classInfo.PluginCategory);
    private static string GetPluginCategory(AudioPluginCategory category)
    {
        return category switch
        {
            AudioPluginCategory.Processor => AudioEffectCategory,
            AudioPluginCategory.Controller => ComponentControllerCategory,
            AudioPluginCategory.TestProvider => "Test Class",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public partial struct IPluginFactory
    {
        private static IAudioPluginFactory Get(IPluginFactory* self) => ((ComObjectHandle*)self)->As<IAudioPluginFactory>();

        private static partial ComResult getFactoryInfo_ToManaged(IPluginFactory* self, PFactoryInfo* info)
        {
            *info = default;
            var factoryInfo = Get(self).FactoryInfo;
            // vendor[64]
            CopyStringToUTF8(factoryInfo.Vendor, info->vendor, 64);
            // url[256]
            CopyStringToUTF8(factoryInfo.Url, info->url, 256);
            // email[128]
            CopyStringToUTF8(factoryInfo.Email, info->email, 128);
            // flags
            info->flags = (int)factoryInfo.Flags | (int)PFactoryInfo.FactoryFlags.kUnicode;
            return true;
        }

        private static partial int countClasses_ToManaged(IPluginFactory* self)
        {
            return Get(self).PluginClassInfoCount;
        }

        private static partial ComResult getClassInfo_ToManaged(IPluginFactory* self, int index, PClassInfo* info)
        {
            var pluginClassInfo = Get(self).GetPluginClassInfo(index);
            info->cid = pluginClassInfo.ClassId;
            info->cardinality = pluginClassInfo.Cardinality;
            CopyStringToUTF8(AudioEffectCategory, info->category, 32);
            CopyStringToUTF8(pluginClassInfo.Name, info->name, 64);
            return true;
        }

        private static partial ComResult createInstance_ToManaged(IPluginFactory* self, LibVst.FIDString cid, LibVst.FIDString _iid, void** obj)
        {
            var comResult = false;
            *obj = null;
            var pluginComponent = Get(self).CreateInstance(*(Guid*)cid.Value);
            if (pluginComponent != null)
            {
                var comObject = ComObjectManager.Instance.GetOrCreateComObject(pluginComponent);
                var nativePointerForRequestedInterfaceIid = comObject.QueryInterface(*(Guid*)_iid.Value);
                if (nativePointerForRequestedInterfaceIid != IntPtr.Zero)
                {
                    *obj = (void*)nativePointerForRequestedInterfaceIid;
                    comResult = true;
                }
                else
                {
                    comObject.Dispose();
                }
            }
            return comResult;
        }
    }
}