// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IComponent
    {
        private static partial ComResult getControllerClassId_ccw(ComObject* self, Guid classId)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setIoMode_ccw(ComObject* self, IoMode mode)
        {
            throw new NotImplementedException();
        }

        private static partial int getBusCount_ccw(ComObject* self, MediaType type, BusDirection dir)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getBusInfo_ccw(ComObject* self, MediaType type, BusDirection dir, int index, BusInfo* bus)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getRoutingInfo_ccw(ComObject* self, RoutingInfo* inInfo, RoutingInfo* outInfo)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult activateBus_ccw(ComObject* self, MediaType type, BusDirection dir, int index, bool state)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setActive_ccw(ComObject* self, bool state)
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
    }
}