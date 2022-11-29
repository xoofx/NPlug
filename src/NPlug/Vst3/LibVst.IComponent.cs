// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IComponent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NPlug.IAudioProcessor Get(ComObject* self)
        {
            return (NPlug.IAudioProcessor)self->Handle.Target!;
        }
        
        private static partial ComResult getControllerClassId_ccw(ComObject* self, Guid* classId)
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

        private static partial ComResult setIoMode_ccw(ComObject* self, IoMode mode)
        {
            try
            {
                Get(self).SetInputOutputMode((AudioInputOutputMode)mode.Value);
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial int getBusCount_ccw(ComObject* self, MediaType type, BusDirection dir)
        {
            try
            {
                return Get(self).GetBusCount((AudioBusMediaType)type.Value, (AudioBusDirection)dir.Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static partial ComResult getBusInfo_ccw(ComObject* self, MediaType type, BusDirection dir, int index, BusInfo* bus)
        {
            try
            {
                var busInfo = Get(self).GetBusInfo((AudioBusMediaType)type.Value, (AudioBusDirection)dir.Value, index);
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

        private static partial ComResult getRoutingInfo_ccw(ComObject* self, RoutingInfo* inInfo, RoutingInfo* outInfo)
        {
            try
            {
                return Get(self).TryGetBusRoutingInfo(in *(AudioBusRoutingInfo*)inInfo, out *(AudioBusRoutingInfo*)outInfo);
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult activateBus_ccw(ComObject* self, MediaType type, BusDirection dir, int index, bool state)
        {
            try
            {
                return Get(self).ActivateBus((AudioBusMediaType)type.Value, (AudioBusDirection)dir.Value, index, state);
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult setActive_ccw(ComObject* self, bool state)
        {
            try
            {
                Get(self).SetActive(state);
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult setState_ccw(ComObject* self, IBStream* state)
        {
            try
            {
                // TODO: cache from AudioHostApplication
                Get(self).SetState(new IBStreamClient() { NativeStream = state });
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult getState_ccw(ComObject* self, IBStream* state)
        {
            try
            {
                // TODO: cache from AudioHostApplication
                Get(self).GetState(new IBStreamClient() { NativeStream = state });
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }
    }
}