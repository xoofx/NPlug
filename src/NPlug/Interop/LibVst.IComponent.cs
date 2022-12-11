// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IComponent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NPlug.IAudioProcessor Get(IComponent* self) => (NPlug.IAudioProcessor)((ComObjectHandle*)self)->Target!;
        
        private static partial ComResult getControllerClassId_ToManaged(IComponent* self, Guid* classId)
        {
            *classId = Get(self).ControllerId;
            return true;
        }

        private static partial ComResult setIoMode_ToManaged(IComponent* self, IoMode mode)
        {
            Get(self).SetInputOutputMode((InputOutputMode)mode.Value);
            return true;
        }

        private static partial int getBusCount_ToManaged(IComponent* self, MediaType type, BusDirection dir)
        {
            return Get(self).GetBusCount((BusMediaType)type.Value, (NPlug.BusDirection)dir.Value);
        }

        private static partial ComResult getBusInfo_ToManaged(IComponent* self, MediaType type, BusDirection dir, int index, BusInfo* bus)
        {
            var busInfo = Get(self).GetBusInfo((BusMediaType)type.Value, (NPlug.BusDirection)dir.Value, index);
            bus->mediaType.Value = (int)busInfo.MediaType;
            bus->direction.Value = (int)busInfo.Direction;
            bus->channelCount = busInfo.ChannelCount;
            bus->name.CopyFrom(busInfo.Name);
            bus->busType.Value = (int)busInfo.BusType;
            bus->flags = (uint)busInfo.Flags;
            return true;
        }

        private static partial ComResult getRoutingInfo_ToManaged(IComponent* self, RoutingInfo* inInfo, RoutingInfo* outInfo)
        {
            return Get(self).TryGetBusRoutingInfo(in *(BusRoutingInfo*)inInfo, out *(BusRoutingInfo*)outInfo);
        }

        private static partial ComResult activateBus_ToManaged(IComponent* self, MediaType type, BusDirection dir, int index, byte state)
        {
            Get(self).ActivateBus((BusMediaType)type.Value, (NPlug.BusDirection)dir.Value, index, state != 0);
            return true;
        }

        private static partial ComResult setActive_ToManaged(IComponent* self, byte state)
        {
            Get(self).SetActive(state != 0);
            return true;
        }

        private static partial ComResult setState_ToManaged(IComponent* self, IBStream* state)
        {
            Get(self).SetState(IBStreamClient.GetStream(state));
            return true;
        }

        private static partial ComResult getState_ToManaged(IComponent* self, IBStream* state)
        {
            Get(self).GetState(IBStreamClient.GetStream(state));
            return true;
        }
    }
}