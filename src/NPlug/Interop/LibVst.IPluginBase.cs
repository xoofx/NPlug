// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPluginBase
    {
        private static IAudioPluginComponent Get(IPluginBase* self) => (IAudioPluginComponent)((ComObjectHandle*)self)->Handle.Target!;

        private static partial ComResult initialize_ToManaged(IPluginBase* self, LibVst.FUnknown* context)
        {
            IHostApplication* hostApplication;
            var result = context->queryInterface(IHostApplication.NativeGuid, (void**)&hostApplication);
            if (result.IsSuccess)
            {
                String128 name = default;
                _ = hostApplication->getName(&name);
                return Get(self).Initialize(new AudioHostApplicationClient(hostApplication, name.ToString()));
            }
            else
            {
                // TODO Free self->Handle
                return false;
            }
        }

        private static partial ComResult terminate_ToManaged(IPluginBase* self)
        {
            Get(self).Terminate();
            return true;
        }
    }
}