// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct FUnknown
    {

        private delegate bool TryQueryInterfaceDelegate(Guid* iid, ComObject bridge, void** pInterface);

        private static ComObjectHandle* Get(FUnknown* self) => (ComObjectHandle*)self;

        // Global map VST internal types to public types
        private static readonly Dictionary<Guid, TryQueryInterfaceDelegate> MapGuidToDelegate = new()
        {
            { IPluginBase.IId, TryMatchQueryInterface<IPluginBase, IAudioPluginComponent> },
            { IComponent.IId, TryMatchQueryInterface<IComponent, NPlug.IAudioProcessor> },
            { IAudioProcessor.IId, TryMatchQueryInterface<IComponent, NPlug.IAudioProcessor> },
            { IConnectionPoint.IId, TryMatchQueryInterface<IComponent, IAudioConnectionPoint> },
            { IPlugView.IId, TryMatchQueryInterface<IComponent, IAudioPluginView> },
        };

        private static partial ComResult queryInterface_ccw(FUnknown* pObj, Guid* iid, void** pInterface)
        {
            *pInterface = (void*)0;
            var bridge = Get(pObj)->ComObject;
            return MapGuidToDelegate.TryGetValue(*iid, out var match) && match(iid, bridge, pInterface) ? ComResult.Ok : ComResult.NoInterface;
        }

        private static bool TryMatchQueryInterface<TNative, TUser>(Guid* iid, ComObject bridge, void** pInterface) where TNative : unmanaged, INativeGuid, INativeVtbl
        {
            if (bridge.Target is TUser)
            {
                *pInterface = (void*)bridge.GetOrCreateComInterface<TNative>();
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