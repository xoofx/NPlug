// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

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
                // TODO
                Get(self).SetControllerHost(new AudioControllerHost());
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
}