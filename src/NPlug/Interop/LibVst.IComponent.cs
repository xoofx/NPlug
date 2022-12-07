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
        private static NPlug.IAudioProcessor Get(IComponent* self)
        {
            return (NPlug.IAudioProcessor)((ComObjectHandle*)self)->Handle.Target!;
        }
        
        private static partial ComResult getControllerClassId_ccw(IComponent* self, Guid* classId)
        {
            try
            {
                *classId = Get(self).ControllerId;
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult setIoMode_ccw(IComponent* self, IoMode mode)
        {
            try
            {
                Get(self).SetInputOutputMode((InputOutputMode)mode.Value);
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial int getBusCount_ccw(IComponent* self, MediaType type, BusDirection dir)
        {
            try
            {
                return Get(self).GetBusCount((BusMediaType)type.Value, (NPlug.BusDirection)dir.Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static partial ComResult getBusInfo_ccw(IComponent* self, MediaType type, BusDirection dir, int index, BusInfo* bus)
        {
            try
            {
                var busInfo = Get(self).GetBusInfo((BusMediaType)type.Value, (NPlug.BusDirection)dir.Value, index);
                bus->mediaType.Value = (int)busInfo.MediaType;
                bus->direction.Value = (int)busInfo.Direction;
                bus->channelCount = busInfo.ChannelCount;
                bus->name.CopyFrom(busInfo.Name);
                bus->busType.Value = (int)busInfo.BusType;
                bus->flags = (uint)busInfo.Flags;
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult getRoutingInfo_ccw(IComponent* self, RoutingInfo* inInfo, RoutingInfo* outInfo)
        {
            try
            {
                return Get(self).TryGetBusRoutingInfo(in *(BusRoutingInfo*)inInfo, out *(BusRoutingInfo*)outInfo);
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult activateBus_ccw(IComponent* self, MediaType type, BusDirection dir, int index, byte state)
        {
            try
            {
                return Get(self).ActivateBus((BusMediaType)type.Value, (NPlug.BusDirection)dir.Value, index, state != 0);
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult setActive_ccw(IComponent* self, byte state)
        {
            try
            {
                Get(self).SetActive(state != 0);
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult setState_ccw(IComponent* self, IBStream* state)
        {
            try
            {
                Get(self).SetState(IBStreamClient.GetStream(state));
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult getState_ccw(IComponent* self, IBStream* state)
        {
            try
            {
                Get(self).GetState(IBStreamClient.GetStream(state));
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }
    }
}