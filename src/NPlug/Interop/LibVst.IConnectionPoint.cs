// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IConnectionPoint
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioConnectionPoint Get(IConnectionPoint* self) => (NPlug.IAudioConnectionPoint)((ComObjectHandle*)self)->Target!;

        private static readonly Dictionary<IntPtr, AudioConnectionPoint> ActiveConnectionPoints = new();

        private static partial ComResult connect_ToManaged(IConnectionPoint* self, IConnectionPoint* other)
        {
            AudioConnectionPoint otherObj;
            lock (ActiveConnectionPoints)
            {
                if (!ActiveConnectionPoints.TryGetValue((IntPtr)other, out otherObj!))
                {
                    otherObj = new AudioConnectionPoint(other);
                    ActiveConnectionPoints.Add((IntPtr)other, otherObj);
                }
            }
            Get(self).Connect(otherObj);
            return true;
        }

        private static partial ComResult disconnect_ToManaged(IConnectionPoint* self, IConnectionPoint* other)
        {
            AudioConnectionPoint otherObj;
            lock (ActiveConnectionPoints)
            {
                if (!ActiveConnectionPoints.TryGetValue((IntPtr)other, out otherObj!))
                {
                    // It should never happen (but a host could choose to)
                    // Don't add it to the dictionary in that case
                    otherObj = new AudioConnectionPoint(other);
                }
            }
            Get(self).Disconnect(otherObj);

            // Remove an active connection
            lock (ActiveConnectionPoints)
            {
                ActiveConnectionPoints.Remove((IntPtr)other);
            }
            return true;
        }

        private static partial ComResult notify_ToManaged(IConnectionPoint* self, IMessage* message)
        {
            var connectionPoint = Get(self);
            // We support only 
            if (connectionPoint is IAudioPluginComponent { Host: AudioHostApplicationClient host })
            {
                var audioMessage = new AudioMessage(host, (IntPtr)message, new AudioAttributeList(host, (IntPtr)message->getAttributes()));
                Get(self).Notify(audioMessage);
                return true;
            }
            else
            {
                return false;
            }
        }

        private class AudioConnectionPoint : IAudioConnectionPoint
        {
            private readonly IConnectionPoint* _nativeConnectionPoint;

            public AudioConnectionPoint(IConnectionPoint* nativeConnectionPoint)
            {
                _nativeConnectionPoint = nativeConnectionPoint;
            }

            public void Connect(IAudioConnectionPoint connectionPoint)
            {
                var comObject = ComObjectManager.Instance.GetOrCreateComObject(connectionPoint);
                var destConnectionPoint = comObject.QueryInterface<IConnectionPoint>();
                _nativeConnectionPoint->connect(destConnectionPoint);
            }

            public void Disconnect(IAudioConnectionPoint connectionPoint)
            {
                var comObject = ComObjectManager.Instance.GetOrCreateComObject(connectionPoint);
                var destConnectionPoint = comObject.QueryInterface<IConnectionPoint>();
                _nativeConnectionPoint->disconnect(destConnectionPoint);
            }

            public void Notify(AudioMessage message)
            {
                _nativeConnectionPoint->notify((IMessage*)message.NativeContext);
            }
        }
    }
}