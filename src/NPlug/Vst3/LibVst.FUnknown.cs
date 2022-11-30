// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct FUnknown
    {
        private static ComObjectHandle* Get(FUnknown* self) => (ComObjectHandle*)self;

        private static partial ComResult queryInterface_ccw(FUnknown* pObj, Guid* iid, void** pInterface)
        {
            *pInterface = (void*)0;
            var bridge = Get(pObj)->ComObject;

            if (TryQueryInterface<IPluginBase, IAudioPlugin>(iid, bridge, pInterface)
                || TryQueryInterface<IComponent, IAudioProcessor>(iid, bridge, pInterface)
                || TryQueryInterface<IAudioProcessor, IAudioProcessor>(iid, bridge, pInterface))
            {
                return ComResult.Ok;
            }
            return ComResult.NoInterface;
        }

        private static bool TryQueryInterface<TNative, TUser>(Guid* iid, ComObject bridge, void** pInterface) where TNative : INativeGuid, INativeVtbl
        {
            if (*iid == *TNative.NativeGuid && bridge.Target is TUser)
            {
                *pInterface = bridge.GetOrComObjectHandle<TNative>();
                return true;
            }
            return false;
        }

        private static partial uint addRef_ccw(FUnknown* pObj)
        {
            return Get(pObj)->ComObject.AddRef();
        }

        private static partial uint release_ccw(FUnknown* pObj)
        {
            return Get(pObj)->ComObject.ReleaseRef();
        }
    }
}