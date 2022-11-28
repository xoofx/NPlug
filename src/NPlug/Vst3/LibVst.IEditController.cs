// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IEditController
    {
        private static partial ComResult setComponentState_ccw(ComObject* self, IBStream* state)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setState_ccw(ComObject* self, IBStream* state)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getState_ccw(ComObject* self, IBStream* state)
        {
            throw new NotImplementedException();
        }

        private static partial int getParameterCount_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getParameterInfo_ccw(ComObject* self, int paramIndex, ParameterInfo* info)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getParamStringByValue_ccw(ComObject* self, ParamID id, ParamValue valueNormalized, String128* @string)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getParamValueByString_ccw(ComObject* self, ParamID id, char* @string, ParamValue* valueNormalized)
        {
            throw new NotImplementedException();
        }

        private static partial ParamValue normalizedParamToPlain_ccw(ComObject* self, ParamID id, ParamValue valueNormalized)
        {
            throw new NotImplementedException();
        }

        private static partial ParamValue plainParamToNormalized_ccw(ComObject* self, ParamID id, ParamValue plainValue)
        {
            throw new NotImplementedException();
        }

        private static partial ParamValue getParamNormalized_ccw(ComObject* self, ParamID id)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setParamNormalized_ccw(ComObject* self, ParamID id, ParamValue value)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setComponentHandler_ccw(ComObject* self, IComponentHandler* handler)
        {
            throw new NotImplementedException();
        }

        private static partial IPlugView* createView_ccw(ComObject* self, FIDString name)
        {
            throw new NotImplementedException();
        }
    }
}