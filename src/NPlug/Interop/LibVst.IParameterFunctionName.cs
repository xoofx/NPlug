// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IParameterFunctionName
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerParameterFunctionName Get(IParameterFunctionName* self) => ((ComObjectHandle*)self)->As<IAudioControllerParameterFunctionName>();

        private static partial ComResult getParameterIDFromFunctionName_ToManaged(IParameterFunctionName* self, LibVst.UnitID unitID, LibVst.FIDString functionName, LibVst.ParamID* paramID)
        {
            var controller = Get(self);
            var host = (AudioHostApplicationClient)controller.Host!;

            return Get(self).TryGetParameterIdFromFunctionName(new AudioUnitId(unitID.Value), host.GetOrCreateString(functionName.Value), out *(AudioParameterId*)paramID);
        }
    }
}
