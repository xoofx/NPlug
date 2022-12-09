// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Reflection;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPluginFactory
    {
        private static IAudioPluginFactory Get(IPluginFactory* self) => ((ComObjectHandle*)self)->As<IAudioPluginFactory>();

        private static partial ComResult getFactoryInfo_ccw(IPluginFactory* self, PFactoryInfo* info)
        {
            try
            {
                var factoryInfo = Get(self).FactoryInfo;
                // vendor[64]
                CopyStringToUTF8(factoryInfo.Vendor, info->vendor, 64);
                // url[256]
                CopyStringToUTF8(factoryInfo.Url, info->url, 256);
                // email[128]
                CopyStringToUTF8(factoryInfo.Email, info->email, 128);
                return true;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private static partial int countClasses_ccw(IPluginFactory* self)
        {
            try
            {
                return Get(self).PluginClassInfoCount;
            }
            catch
            {
                return 0;
            }
        }

        private static partial ComResult getClassInfo_ccw(IPluginFactory* self, int index, PClassInfo* info)
        {
            try
            {
                var pluginClassInfo = Get(self).GetPluginClassInfo(index);
                info->cid = pluginClassInfo.Id;
                info->cardinality = pluginClassInfo.Cardinality;
                CopyStringToUTF8(AudioEffectCategory, info->category, 32);
                CopyStringToUTF8(pluginClassInfo.Name, info->name, 64);
                return true;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private static partial ComResult createInstance_ccw(IPluginFactory* self, FIDString cid, FIDString iid, void** obj)
        {
            var comResult = false;
            try
            {
                var pluginComponent = Get(self).CreateInstance(*(Guid*)cid.Value);
                if (pluginComponent != null)
                {
                    var comObject = ComObjectManager.Instance.GetOrCreateComObject(pluginComponent);
                    var nativePointerForRequestedInterfaceIid = comObject.GetOrCreateComInterface(*(Guid*)iid.Value);
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
            }
            catch (Exception ex)
            {
                return ex;
            }

            return comResult;
        }
    }
}