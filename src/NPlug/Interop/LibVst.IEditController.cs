// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NPlug.Backend;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IEditController
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NPlug.IAudioController Get(IEditController* self) => ((ComObjectHandle*)self)->As<NPlug.IAudioController>();

        private static partial ComResult setComponentState_ToManaged(IEditController* self, IBStream* state)
        {
            Get(self).SetComponentState(IBStreamClient.GetStream(state));
            return true;
        }

        private static partial ComResult setState_ToManaged(IEditController* self, IBStream* state)
        {
            Get(self).SetState(IBStreamClient.GetStream(state));
            return true;
        }

        private static partial ComResult getState_ToManaged(IEditController* self, IBStream* state)
        {
            Get(self).GetState(IBStreamClient.GetStream(state));
            return true;
        }

        private static partial int getParameterCount_ToManaged(IEditController* self)
        {
            return Get(self).ParameterCount;
        }

        private static partial ComResult getParameterInfo_ToManaged(IEditController* self, int paramIndex, ParameterInfo* info)
        {
            var parameter = Get(self).GetParameterInfo(paramIndex);
            info->id = parameter.Id;
            info->title.CopyFrom(parameter.Title);
            info->shortTitle.CopyFrom(parameter.ShortTitle);
            info->units.CopyFrom(parameter.Units);
            info->stepCount = parameter.StepCount;
            info->defaultNormalizedValue = parameter.DefaultNormalizedValue;
            info->unitId = parameter.UnitId;
            info->flags = (int)parameter.Flags;
            return true;
        }

        private static partial ComResult getParamStringByValue_ToManaged(IEditController* self, ParamID id, ParamValue valueNormalized, String128* @string)
        {
            var stringResult = Get(self).GetParameterStringByValue(id, valueNormalized.Value);
            @string->CopyFrom(stringResult);
            return true;
        }

        private static partial ComResult getParamValueByString_ToManaged(IEditController* self, ParamID id, char* @string, ParamValue* valueNormalized)
        {
            var audioProcessor = Get(self);
            var host = (AudioHostApplicationClient)audioProcessor.Host!;
            valueNormalized->Value = Get(self).GetParameterValueByString(id, host.GetOrCreateString128(@string));
            return true;
        }

        private static partial ParamValue normalizedParamToPlain_ToManaged(IEditController* self, ParamID id, ParamValue valueNormalized)
        {
            return new ParamValue(Get(self).NormalizedParameterToPlain(id, valueNormalized.Value));
        }

        private static partial ParamValue plainParamToNormalized_ToManaged(IEditController* self, ParamID id, ParamValue plainValue)
        {
            return new ParamValue(Get(self).NormalizedParameterToPlain(id, plainValue.Value));
        }

        private static partial ParamValue getParamNormalized_ToManaged(IEditController* self, ParamID id)
        {
            return new ParamValue(Get(self).GetParameterNormalized(id));
        }

        private static partial ComResult setParamNormalized_ToManaged(IEditController* self, ParamID id, ParamValue value)
        {
            Get(self).SetParameterNormalized(id, value.Value);
            return true;
        }

        private static partial ComResult setComponentHandler_ToManaged(IEditController* self, IComponentHandler* handler)
        {
            Get(self).SetControllerHandler(handler == null ? null : new AudioControllerHandlerProxy(handler));
            return true;
        }

        private static partial IPlugView* createView_ToManaged(IEditController* self, FIDString name)
        {
            var audioProcessor = Get(self);
            var view = Get(self).CreateView();
            if (view is null)
            {
                return null;
            }
            var comObject = ComObjectManager.Instance.GetOrCreateComObject(view);
            return comObject.QueryInterface<IPlugView>();
        }
    }

    /// <summary>
    /// Access to all the optional native handlers.
    /// </summary>
    private class AudioControllerHandlerProxy : IAudioControllerHandler
    {
        private readonly IComponentHandler* _handler;
        private readonly IComponentHandler2* _handler2;
        private readonly IComponentHandler3* _handler3;
        private readonly IComponentHandlerBusActivation* _busActivation;
        private readonly IProgress* _progress;
        private readonly IUnitHandler* _unitHandler;

        public AudioControllerHandlerProxy(IComponentHandler* handler)
        {
            _handler = handler;
            _handler2 = QueryInterface<IComponentHandler, IComponentHandler2>(_handler);
            _handler3 = QueryInterface<IComponentHandler, IComponentHandler3>(_handler);
            _busActivation = QueryInterface<IComponentHandler, IComponentHandlerBusActivation>(_handler);
            _progress = QueryInterface<IComponentHandler, IProgress>(_handler);
            _unitHandler = QueryInterface<IComponentHandler, IUnitHandler>(_handler);
        }
        
        public void BeginEdit(AudioParameterId id)
        {
            _handler->beginEdit(id);
        }

        public void PerformEdit(AudioParameterId id, double valueNormalized)
        {
            _handler->performEdit(id, valueNormalized);
        }

        public void EndEdit(AudioParameterId id)
        {
            _handler->endEdit(id);
        }

        public void RestartComponent(AudioRestartFlags flags)
        {
            _handler->restartComponent((int)flags);
        }

        public bool IsAdvancedEditSupported => _handler2 != null;

        public void SetDirty(bool state)
        {
            ThrowIfNotIsAdvancedEditSupported();
            _handler2->setDirty(state ? (byte)1 : (byte)0);
        }

        public void RequestOpenEditor(string name)
        {
            ThrowIfNotIsAdvancedEditSupported();
            using var tempUtf8 = new TempUtf8String(name);
            fixed (byte* pBuffer = tempUtf8.Buffer)
            {
                _handler2->requestOpenEditor(new FIDString() { Value = pBuffer });
            }
        }

        public void StartGroupEdit()
        {
            ThrowIfNotIsAdvancedEditSupported();
            _handler2->startGroupEdit();
        }

        public void FinishGroupEdit()
        {
            ThrowIfNotIsAdvancedEditSupported();
            _handler2->finishGroupEdit();
        }

        private void ThrowIfNotIsAdvancedEditSupported()
        {
            if (!IsAdvancedEditSupported) throw new NotSupportedException($"This method is not supported because {nameof(IsAdvancedEditSupported)} is false");
        }

        public bool IsCreateContextMenuSupported => _handler3 != null;

        public IAudioContextMenu CreateContextMenu(IAudioPluginView plugView, AudioParameterId paramID)
        {
            ThrowIfNotIsCreateContextMenuSupported();

            var comObject = ComObjectManager.Instance.GetOrCreateComObject(plugView);
            var nativePlugView = comObject.QueryInterface<IPlugView>();
            var nativeContextMenu = _handler3->createContextMenu(nativePlugView, (ParamID*)&paramID);
            var audioContextMenu = new AudioContextMenuVst(nativeContextMenu);
            return audioContextMenu;
        }

        private void ThrowIfNotIsCreateContextMenuSupported()
        {
            if (!IsCreateContextMenuSupported) throw new NotSupportedException($"This method is not supported because {nameof(IsCreateContextMenuSupported)} is false");
        }

        public bool IsRequestBusActivationSupported => _busActivation != null;

        public void RequestBusActivation(BusMediaType type, NPlug.BusDirection dir, int index, bool state)
        {
            ThrowIfNotIsRequestBusActivationSupported();
            _busActivation->requestBusActivation(new MediaType((int)type), new BusDirection((int)dir), index, state ? (byte)1 : (byte)0);
        }

        private void ThrowIfNotIsRequestBusActivationSupported()
        {
            if (!IsRequestBusActivationSupported) throw new NotSupportedException($"This method is not supported because {nameof(IsRequestBusActivationSupported)} is false");
        }

        public bool IsProgressSupported => _progress != null;

        public AudioProgressId StartProgress(AudioProgressType type, string? optionalDescription)
        {
            ThrowIfNotIsProgressSupported();
            fixed (char* pOpt = optionalDescription)
            {
                AudioProgressId id = default;
                _progress->start((IProgress.ProgressType)type, pOpt, (ID*)&id);
                return id;
            }
        }

        public void UpdateProgress(AudioProgressId id, double normValue)
        {
            ThrowIfNotIsProgressSupported();
            _progress->update(new ID(id.Value), normValue);
        }

        public void FinishProgress(AudioProgressId id)
        {
            ThrowIfNotIsProgressSupported();
            _progress->finish(new ID(id.Value));
        }

        private void ThrowIfNotIsProgressSupported()
        {
            if (!IsProgressSupported) throw new NotSupportedException($"This method is not supported because {nameof(IsProgressSupported)} is false");
        }

        public bool IsUnitAndProgramListSupported => _unitHandler != null;

        public void NotifyUnitSelection(AudioUnitId unitId)
        {
            ThrowIfNotIsUnitAndProgramListSupported();
            _unitHandler->notifyUnitSelection(new UnitID(unitId.Value));
        }

        public void NotifyProgramListChange(AudioProgramListId listId, int programIndex)
        {
            ThrowIfNotIsUnitAndProgramListSupported();
            _unitHandler->notifyProgramListChange(new ProgramListID(listId.Value), programIndex);
        }
        
        private void ThrowIfNotIsUnitAndProgramListSupported()
        {
            if (!IsUnitAndProgramListSupported) throw new NotSupportedException($"This method is not supported because {nameof(IsUnitAndProgramListSupported)} is false");
        }
    }

    private sealed class AudioContextMenuVst : IAudioContextMenu
    {
        private readonly IContextMenu* _contextMenu;
        private readonly List<(AudioContextMenuItem, nint, AudioContextMenuAction?)> _items;
        private bool _disposed;
        
        public AudioContextMenuVst(IContextMenu* contextMenu)
        {
            _contextMenu = contextMenu;
            _items = new List<(AudioContextMenuItem, nint, AudioContextMenuAction?)>();
        }
        
        public int GetItemCount()
        {
            return _contextMenu->getItemCount();
        }

        public void GetItem(int index, out AudioContextMenuItem item, out AudioContextMenuAction? target)
        {
            // We are using our internal list
            var tuple = _items[index];
            item = tuple.Item1;
            target = tuple.Item3;
            //item = new AudioContextMenuItem(string.Empty);
            //target = null;
            //Item nativeItem;
            //IContextMenuTarget* nativeTarget;
            //if (_contextMenu->getItem(index, &nativeItem, &nativeTarget))
            //{
            //    item = ConvertTo(nativeItem.Value);
            //    target = tag => nativeTarget->executeMenuItem(tag);
            //}
        }

        public void AddItem(in AudioContextMenuItem item, AudioContextMenuAction? target)
        {
            var nativeItem = ConvertFrom(item);
            ComObject? comObject = null;
            IContextMenuTarget* nativeTarget = null;
            if (target is not null)
            {
                comObject = ComObjectManager.Instance.GetOrCreateComObject(target);
                nativeTarget = comObject.QueryInterface<IContextMenuTarget>();
            }
            if (_contextMenu->addItem((Item*)&nativeItem, nativeTarget))
            {
                _items.Add((item, (nint)nativeTarget, target));
            }
            else
            {
                // Release the native target
                comObject?.ReleaseRef();
            }
        }

        public void RemoveItem(in AudioContextMenuItem item, AudioContextMenuAction? target)
        {
            var nativeItem = ConvertFrom(item);
            foreach (var tuple in _items)
            {
                if (tuple.Item1 == item && tuple.Item3 == target)
                {
                    _contextMenu->removeItem((Item*)&nativeItem, (IContextMenuTarget*)tuple.Item2);
                    break;
                }
            }
        }

        public void Popup(int x, int y)
        {
            _contextMenu->popup(new UCoord(x), new UCoord(y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IContextMenuItem ConvertFrom(in AudioContextMenuItem item)
        {
            var nativeItem = new IContextMenuItem();
            nativeItem.name.CopyFrom(item.Name);
            nativeItem.tag = item.Tag;
            nativeItem.flags = (int)item.Flags;
            return nativeItem;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static AudioContextMenuItem ConvertTo(in IContextMenuItem item)
        //{
        //    var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in item.name.Value[0]), 128);
        //    span = span.Slice(0, span.IndexOf((char)0));
        //    return new AudioContextMenuItem(new string(span), item.tag, (AudioContextMenuItemFlags)item.flags);
        //}
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            foreach (var item in _items)
            {
                ((IContextMenuTarget*)item.Item2)->release();
            }

            _items.Clear();
            _contextMenu->release();
        }
    }
}