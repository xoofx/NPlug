// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NPlug.Backend;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IEditController
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NPlug.IAudioController Get(IEditController* self) => (NPlug.IAudioController)((ComObjectHandle*)self)->Handle.Target!;

        private static partial ComResult setComponentState_ccw(IEditController* self, IBStream* state)
        {
            try
            {
                Get(self).SetComponentState(IBStreamClient.GetStream(state));
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult setState_ccw(IEditController* self, IBStream* state)
        {
            try
            {
                Get(self).SetState(IBStreamClient.GetStream(state));
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult getState_ccw(IEditController* self, IBStream* state)
        {
            try
            {
                Get(self).GetState(IBStreamClient.GetStream(state));
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.InternalError;
            }
        }

        private static partial int getParameterCount_ccw(IEditController* self)
        {
            try
            {
                return Get(self).ParameterCount;
            }
            catch
            {
                return 0;
            }
        }

        private static partial ComResult getParameterInfo_ccw(IEditController* self, int paramIndex, ParameterInfo* info)
        {
            try
            {
                var parameter = Get(self).GetParameterInfo(paramIndex);
                info->id = new ParamID(unchecked((uint)parameter.Id.Value));
                info->title.CopyFrom(parameter.Title);
                info->shortTitle.CopyFrom(parameter.ShortTitle);
                info->units.CopyFrom(parameter.Units);
                info->stepCount = parameter.StepCount;
                info->defaultNormalizedValue = new ParamValue(parameter.DefaultNormalizedValue);
                info->unitId = new UnitID(parameter.UnitId.Value);
                info->flags = (int)parameter.Flags;
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.False;
            }
        }

        private static partial ComResult getParamStringByValue_ccw(IEditController* self, ParamID id, ParamValue valueNormalized, String128* @string)
        {
            try
            {
                var stringResult = Get(self).GetParameterStringByValue(new AudioParameterId(unchecked((int)id.Value)), valueNormalized.Value);
                @string->CopyFrom(stringResult);
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.False;
            }
        }

        private static partial ComResult getParamValueByString_ccw(IEditController* self, ParamID id, char* @string, ParamValue* valueNormalized)
        {
            try
            {
                var audioProcessor = Get(self);
                var host = (AudioHostApplicationClient)audioProcessor.Host!;
                valueNormalized->Value = Get(self).GetParameterValueByString(new AudioParameterId(unchecked((int)id.Value)), host.GetOrCreateString128(@string));
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.False;
            }
        }

        private static partial ParamValue normalizedParamToPlain_ccw(IEditController* self, ParamID id, ParamValue valueNormalized)
        {
            try
            {
                return new ParamValue(Get(self).NormalizedParameterToPlain(new AudioParameterId(unchecked((int)id.Value)), valueNormalized.Value));
            }
            catch
            {
                return new ParamValue(0.0);
            }
        }

        private static partial ParamValue plainParamToNormalized_ccw(IEditController* self, ParamID id, ParamValue plainValue)
        {
            try
            {
                return new ParamValue(Get(self).NormalizedParameterToPlain(new AudioParameterId(unchecked((int)id.Value)), plainValue.Value));
            }
            catch
            {
                return new ParamValue(0.0);
            }
        }

        private static partial ParamValue getParamNormalized_ccw(IEditController* self, ParamID id)
        {
            try
            {
                return new ParamValue(Get(self).GetParameterNormalized(new AudioParameterId(unchecked((int)id.Value))));
            }
            catch
            {
                return new ParamValue(0.0);
            }
        }

        private static partial ComResult setParamNormalized_ccw(IEditController* self, ParamID id, ParamValue value)
        {
            try
            {
                Get(self).SetParameterNormalized(new AudioParameterId(unchecked((int)id.Value)), value.Value);
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.False;
            }
        }

        private static partial ComResult setComponentHandler_ccw(IEditController* self, IComponentHandler* handler)
        {
            try
            {
                Get(self).SetControllerHost(new AudioControllerHostProxy(handler));
                return ComResult.Ok;
            }
            catch
            {
                return ComResult.False;
            }
        }

        private static partial IPlugView* createView_ccw(IEditController* self, FIDString name)
        {
            try
            {
                var audioProcessor = Get(self);
                var host = (AudioHostApplicationClient)audioProcessor.Host!;
                var view = Get(self).CreateView(host.GetOrCreateString(name.Value));
                var comObject = ComObjectManager.Instance.GetOrCreateComObject(view);
                return comObject.GetOrCreateComInterface<IPlugView>();
            }
            catch
            {
                return (IPlugView*)0;
            }
        }
    }

    /// <summary>
    /// Access to all the optional native handlers.
    /// </summary>
    private class AudioControllerHostProxy : AudioControllerHost
    {
        private readonly IComponentHandler* _handler;
        private readonly IComponentHandler2* _handler2;
        private readonly IComponentHandler3* _handler3;
        private readonly IComponentHandlerBusActivation* _busActivation;
        private readonly IProgress* _progress;
        private readonly IUnitHandler* _unitHandler;

        public AudioControllerHostProxy(IComponentHandler* handler)
        {
            _handler = handler;

            var pUnk = (void*)0;
            if (_handler->queryInterface(IComponentHandler2.NativeGuid, &pUnk))
            {
                _handler2 = (IComponentHandler2*)pUnk;
            }

            pUnk = (void*)0;
            if (_handler->queryInterface(IComponentHandler3.NativeGuid, &pUnk))
            {
                _handler3 = (IComponentHandler3*)pUnk;
            }

            pUnk = (void*)0;
            if (_handler->queryInterface(IComponentHandlerBusActivation.NativeGuid, &pUnk))
            {
                _busActivation = (IComponentHandlerBusActivation*)pUnk;
            }

            pUnk = (void*)0;
            if (_handler->queryInterface(IProgress.NativeGuid, &pUnk))
            {
                _progress = (IProgress*)pUnk;
            }

            pUnk = (void*)0;
            if (_handler->queryInterface(IUnitHandler.NativeGuid, &pUnk))
            {
                _unitHandler = (IUnitHandler*)pUnk;
            }
        }
        
        public override void BeginEdit(AudioParameterId id)
        {
            _handler->beginEdit(id);
        }

        public override void PerformEdit(AudioParameterId id, double valueNormalized)
        {
            _handler->performEdit(id, valueNormalized);
        }

        public override void EndEdit(AudioParameterId id)
        {
            _handler->endEdit(id);
        }

        public override void RestartComponent(AudioRestartFlags flags)
        {
            _handler->restartComponent((int)flags);
        }

        public override bool IsAdvancedEditSupported => _handler2 != null;

        public override void SetDirty(bool state)
        {
            ThrowIfNotIsAdvancedEditSupported();
            _handler2->setDirty(state ? (byte)1 : (byte)0);
        }

        public override void RequestOpenEditor(string name)
        {
            ThrowIfNotIsAdvancedEditSupported();
            using var tempUtf8 = new TempUtf8String(name);
            fixed (byte* pBuffer = tempUtf8.Buffer)
            {
                _handler2->requestOpenEditor(new FIDString() { Value = pBuffer });
            }
        }

        public override void StartGroupEdit()
        {
            ThrowIfNotIsAdvancedEditSupported();
            _handler2->startGroupEdit();
        }

        public override void FinishGroupEdit()
        {
            ThrowIfNotIsAdvancedEditSupported();
            _handler2->finishGroupEdit();
        }

        private void ThrowIfNotIsAdvancedEditSupported()
        {
            if (!IsAdvancedEditSupported) throw new NotSupportedException($"This method is not supported because {nameof(IsAdvancedEditSupported)} is false");
        }

        public override bool IsCreateContextMenuSupported => _handler3 != null;

        public override AudioContextMenu CreateContextMenu(IAudioPluginView plugView, AudioParameterId paramID)
        {
            ThrowIfNotIsCreateContextMenuSupported();

            var comObject = ComObjectManager.Instance.GetOrCreateComObject(plugView);
            var nativePlugView = comObject.GetOrCreateComInterface<IPlugView>();
            var nativeContextMenu = _handler3->createContextMenu(nativePlugView, (ParamID*)&paramID);
            var audioContextMenu = new AudioContextMenu(AudioContextMenuBackendVst.Instance, (IntPtr)nativeContextMenu);
            return audioContextMenu;
        }

        private void ThrowIfNotIsCreateContextMenuSupported()
        {
            if (!IsCreateContextMenuSupported) throw new NotSupportedException($"This method is not supported because {nameof(IsCreateContextMenuSupported)} is false");
        }

        public override bool IsRequestBusActivationSupported => _busActivation != null;

        public override void RequestBusActivation(AudioBusMediaType type, AudioBusDirection dir, int index, bool state)
        {
            ThrowIfNotIsRequestBusActivationSupported();
            _busActivation->requestBusActivation(new MediaType((int)type), new BusDirection((int)dir), index, state ? (byte)1 : (byte)0);
        }

        private void ThrowIfNotIsRequestBusActivationSupported()
        {
            if (!IsRequestBusActivationSupported) throw new NotSupportedException($"This method is not supported because {nameof(IsRequestBusActivationSupported)} is false");
        }

        public override bool IsProgressSupported => _progress != null;

        public override AudioProgressId StartProgress(AudioProgressType type, string? optionalDescription)
        {
            ThrowIfNotIsProgressSupported();
            fixed (char* pOpt = optionalDescription)
            {
                AudioProgressId id = default;
                _progress->start((IProgress.ProgressType)type, pOpt, (ID*)&id);
                return id;
            }
        }

        public override void UpdateProgress(AudioProgressId id, double normValue)
        {
            ThrowIfNotIsProgressSupported();
            _progress->update(new ID(id.Value), normValue);
        }

        public override void FinishProgress(AudioProgressId id)
        {
            ThrowIfNotIsProgressSupported();
            _progress->finish(new ID(id.Value));
        }

        private void ThrowIfNotIsProgressSupported()
        {
            if (!IsProgressSupported) throw new NotSupportedException($"This method is not supported because {nameof(IsProgressSupported)} is false");
        }

        public override bool IsUnitAndProgramListSupported => _unitHandler != null;

        public override void NotifyUnitSelection(AudioUnitId unitId)
        {
            ThrowIfNotIsUnitAndProgramListSupported();
            _unitHandler->notifyUnitSelection(new UnitID(unitId.Value));
        }

        public override void NotifyProgramListChange(AudioProgramListId listId, int programIndex)
        {
            ThrowIfNotIsUnitAndProgramListSupported();
            _unitHandler->notifyProgramListChange(new ProgramListID(listId.Value), programIndex);
        }
        
        private void ThrowIfNotIsUnitAndProgramListSupported()
        {
            if (!IsUnitAndProgramListSupported) throw new NotSupportedException($"This method is not supported because {nameof(IsUnitAndProgramListSupported)} is false");
        }
    }

    private class AudioContextMenuBackendVst : IAudioContextMenuBackend
    {
        public static readonly AudioContextMenuBackendVst Instance = new AudioContextMenuBackendVst();

        public int GetItemCount(in AudioContextMenu contextMenu)
        {
            return Get(contextMenu)->getItemCount();
        }

        public void GetItem(in AudioContextMenu contextMenu, int index, out AudioContextMenuItem item, out AudioContextMenuAction? target)
        {
            item = new AudioContextMenuItem(string.Empty);
            target = null;
            Item nativeItem;
            IContextMenuTarget* nativeTarget;
            if (Get(contextMenu)->getItem(index, &nativeItem, &nativeTarget))
            {
                item = ConvertTo(nativeItem.Value);
                // TODO: add target
            }
        }

        public void AddItem(in AudioContextMenu contextMenu, in AudioContextMenuItem item, AudioContextMenuAction target)
        {
            var nativeItem = ConvertFrom(item);

            // TODO: add the target
            Get(contextMenu)->addItem((Item*)&nativeItem, null);
        }

        public void RemoveItem(in AudioContextMenu contextMenu, in AudioContextMenuItem item, AudioContextMenuAction target)
        {
            var nativeItem = ConvertFrom(item);
            // TODO: add the target
            Get(contextMenu)->removeItem((Item*)&nativeItem, null);
        }

        public void Popup(in AudioContextMenu contextMenu, int x, int y)
        {
            Get(contextMenu)->popup(new UCoord(x), new UCoord(y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IContextMenu* Get(in AudioContextMenu contextMenu) => (IContextMenu*)contextMenu.NativeContext;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IContextMenuItem ConvertFrom(in AudioContextMenuItem item)
        {
            var nativeItem = new IContextMenuItem();
            nativeItem.name.CopyFrom(item.Name);
            nativeItem.tag = item.Tag;
            nativeItem.flags = (int)item.Flags;
            return nativeItem;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static AudioContextMenuItem ConvertTo(in IContextMenuItem item)
        {
            var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in item.name.Value[0]), 128);
            span = span.Slice(0, span.IndexOf((char)0));
            return new AudioContextMenuItem(new string(span), item.tag, (AudioContextMenuItemFlags)item.flags);
        }
    }
}