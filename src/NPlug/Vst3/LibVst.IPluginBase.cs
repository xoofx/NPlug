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
                // TODO: bind context to AudioHost
                ((AudioPlugin)self->Handle.Target!).Initialize(new AudioHost());
                return ComResult.Ok;
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
                ((AudioPlugin)self->Handle.Target!).Terminate();
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.InternalError;
            }
        }
    }
}