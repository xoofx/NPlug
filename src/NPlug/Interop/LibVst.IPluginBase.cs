// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPluginBase
    {
        private static IAudioPluginComponent Get(IPluginBase* self) => ((ComObjectHandle*)self)->As<IAudioPluginComponent>();

        private static partial ComResult initialize_ToManaged(IPluginBase* self, LibVst.FUnknown* context)
        {
            var hostApplication = QueryInterface<FUnknown, IHostApplication>(context);
            if (hostApplication != null)
            {
                String128 name = default;
                _ = hostApplication->getName(&name);
                return Get(self).Initialize(new AudioHostApplicationClient(hostApplication, name.ToString()));
            }
            else
            {
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