// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IPluginBase
    {
        private static partial ComResult initialize_ccw(ComObject* self, LibVst.FUnknown* context)
        {
            try
            {
                IHostApplication* hostApplication;
                var result = context->queryInterface(IHostApplication.IId, (void**)&hostApplication);
                if (result.IsSuccess)
                {
                    String128 name = default;
                    _ = hostApplication->getName(&name);
                    return ((IAudioPlugin)self->Handle.Target!).Initialize(new AudioHostApplicationVst(hostApplication, name.ToString()));
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult terminate_ccw(ComObject* self)
        {
            try
            {
                ((IAudioPlugin)self->Handle.Target!).Terminate();
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.InternalError;
            }
        }
    }
}