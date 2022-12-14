// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IParameterFinder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioPluginView Get(IParameterFinder* self) => ((ComObjectHandle*)self)->As<IAudioPluginView>();

        private static partial ComResult findParameter_ToManaged(IParameterFinder* self, int xPos, int yPos, LibVst.ParamID* resultTag)
        {
            return Get(self).TryFindParameter(xPos, yPos, out *((AudioParameterId*)resultTag));
        }
    }
}
