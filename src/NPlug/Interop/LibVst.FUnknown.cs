// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace NPlug.Interop;

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
            { IAudioProcessor.IId, TryMatchQueryInterface<IAudioProcessor, NPlug.IAudioProcessor> },
            { IEditController.IId, TryMatchQueryInterface<IEditController, NPlug.IAudioController> },
            { IConnectionPoint.IId, TryMatchQueryInterface<IConnectionPoint, IAudioConnectionPoint> },
            { IPlugView.IId, TryMatchQueryInterface<IPlugView, IAudioPluginView> },
            { IPluginFactory.IId, TryMatchQueryInterface<IPluginFactory, IAudioPluginFactory> },
            { IPluginFactory2.IId, TryMatchQueryInterface<IPluginFactory2, IAudioPluginFactory> },
            { IPluginFactory3.IId, TryMatchQueryInterface<IPluginFactory3, IAudioPluginFactory> },
        };

        private static partial ComResult queryInterface_ToManaged(FUnknown* pObj, Guid* iid, void** pInterface)
        {
            *pInterface = (void*)0;
            var bridge = Get(pObj)->ComObject;
            return MapGuidToDelegate.TryGetValue(*iid, out var match) && match(iid, bridge, pInterface) ? ComResult.Ok : ComResult.NoInterface;
        }

        private static bool TryMatchQueryInterface<TNative, TUser>(Guid* iid, ComObject comObject, void** pInterface) where TNative : unmanaged, INativeGuid, INativeVtbl
        {
            if (comObject.Target is TUser)
            {
                *pInterface = (void*)comObject.QueryInterface<TNative>();
                return true;
            }
            return false;
        }

        private static partial uint addRef_ToManaged(FUnknown* pObj)
        {
            return Get(pObj)->ComObject.AddRef();
        }

        private static partial uint release_ToManaged(FUnknown* pObj)
        {
            return Get(pObj)->ComObject.ReleaseRef();
        }
    }
}