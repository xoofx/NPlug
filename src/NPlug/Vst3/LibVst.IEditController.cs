// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IEditController
    {
        private static partial ComResult setComponentState_ccw(IEditController* self, IBStream* state)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setState_ccw(IEditController* self, IBStream* state)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getState_ccw(IEditController* self, IBStream* state)
        {
            throw new NotImplementedException();
        }

        private static partial int getParameterCount_ccw(IEditController* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getParameterInfo_ccw(IEditController* self, int paramIndex, ParameterInfo* info)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getParamStringByValue_ccw(IEditController* self, ParamID id, ParamValue valueNormalized, String128* @string)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getParamValueByString_ccw(IEditController* self, ParamID id, char* @string, ParamValue* valueNormalized)
        {
            throw new NotImplementedException();
        }

        private static partial ParamValue normalizedParamToPlain_ccw(IEditController* self, ParamID id, ParamValue valueNormalized)
        {
            throw new NotImplementedException();
        }

        private static partial ParamValue plainParamToNormalized_ccw(IEditController* self, ParamID id, ParamValue plainValue)
        {
            throw new NotImplementedException();
        }

        private static partial ParamValue getParamNormalized_ccw(IEditController* self, ParamID id)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setParamNormalized_ccw(IEditController* self, ParamID id, ParamValue value)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setComponentHandler_ccw(IEditController* self, IComponentHandler* handler)
        {
            throw new NotImplementedException();
        }

        private static partial IPlugView* createView_ccw(IEditController* self, FIDString name)
        {
            throw new NotImplementedException();
        }
    }
}