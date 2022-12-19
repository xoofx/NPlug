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
            { IAudioPresentationLatency.IId, TryMatchQueryInterface<IAudioPresentationLatency, IAudioProcessor> },
            { IAudioProcessor.IId, TryMatchQueryInterface<IAudioProcessor, NPlug.IAudioProcessor> },
            { IAutomationState.IId, TryMatchQueryInterface<IAutomationState, IAudioControllerAutomationState> },
            { IComponent.IId, TryMatchQueryInterface<IComponent, NPlug.IAudioProcessor> },
            { IConnectionPoint.IId, TryMatchQueryInterface<IConnectionPoint, IAudioConnectionPoint> },
            { IContextMenuTarget.IId, TryMatchQueryInterface<IContextMenuTarget, System.Delegate> },
            { IEditController.IId, TryMatchQueryInterface<IEditController, NPlug.IAudioController> },
            { IEditController2.IId, TryMatchQueryInterface<IEditController2, IAudioControllerExtended> },
            { IEditControllerHostEditing.IId, TryMatchQueryInterface<IEditControllerHostEditing, IAudioControllerHostEditing> },
            { IInfoListener.IId, TryMatchQueryInterface<IInfoListener, IAudioControllerInfoListener> },
            { IInterAppAudioPresetManager.IId, TryMatchQueryInterface<IInterAppAudioPresetManager, IAudioControllerInterAppAudioPresetManager> },
            { IKeyswitchController.IId, TryMatchQueryInterface<IKeyswitchController, IAudioControllerKeySwitch> },
            { IMidiLearn.IId, TryMatchQueryInterface<IMidiLearn, IAudioControllerMidiLearn> },
            { IMidiMapping.IId, TryMatchQueryInterface<IMidiMapping, IAudioControllerMidiMapping> },
            { INoteExpressionController.IId, TryMatchQueryInterface<INoteExpressionController, IAudioControllerNoteExpression> },
            { INoteExpressionPhysicalUIMapping.IId, TryMatchQueryInterface<INoteExpressionPhysicalUIMapping, IAudioControllerNoteExpressionPhysicalUIMapping> },
            { IParameterFinder.IId, TryMatchQueryInterface<IParameterFinder , IAudioPluginView> },
            { IParameterFunctionName.IId, TryMatchQueryInterface<IParameterFunctionName, IAudioControllerParameterFunctionName> },
            { IPluginBase.IId, TryMatchQueryInterface<IPluginBase, IAudioPluginComponent> },
            { IPluginFactory.IId, TryMatchQueryInterface<IPluginFactory, IAudioPluginFactory> },
            { IPluginFactory2.IId, TryMatchQueryInterface<IPluginFactory2, IAudioPluginFactory> },
            { IPluginFactory3.IId, TryMatchQueryInterface<IPluginFactory3, IAudioPluginFactory> },
            { IPlugView.IId, TryMatchQueryInterface<IPlugView, IAudioPluginView> },
            { IPlugViewContentScaleSupport.IId, TryMatchQueryInterface<IPlugViewContentScaleSupport, IAudioPluginView> },
            { IPrefetchableSupport.IId, TryMatchQueryInterface<IPrefetchableSupport, IAudioProcessorPrefetchable> },
            { IProcessContextRequirements.IId, TryMatchQueryInterface<IProcessContextRequirements, NPlug.IAudioProcessor> },
            { IProgramListData.IId, TryMatchQueryInterface<IProgramListData, IAudioProcessorProgramListData> },
            { ITestPlugProvider.IId, TryMatchQueryInterface<ITestPlugProvider, IAudioTestProvider> },
            { ITestPlugProvider2.IId, TryMatchQueryInterface<ITestPlugProvider2, IAudioTestProvider> },
            { IUnitData.IId, TryMatchQueryInterface<IUnitData, IAudioProcessorUnitData> },
            { IUnitInfo.IId, TryMatchQueryInterface<IUnitInfo, IAudioControllerUnitInfo> },
            { IXmlRepresentationController.IId, TryMatchQueryInterface<IXmlRepresentationController, IAudioControllerXmlRepresentation> },
        };

        private static partial ComResult queryInterface_ToManaged(FUnknown* self, Guid* _iid, void** obj)
        {
            *obj = (void*)0;
            var bridge = Get(self)->ComObject;
            return MapGuidToDelegate.TryGetValue(*_iid, out var match) && match(_iid, bridge, obj) ? ComResult.Ok : ComResult.NoInterface;
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

        private static partial uint addRef_ToManaged(FUnknown* self)
        {
            return Get(self)->ComObject.AddRef();
        }

        private static partial uint release_ToManaged(FUnknown* self)
        {
            return Get(self)->ComObject.ReleaseRef();
        }
    }
}