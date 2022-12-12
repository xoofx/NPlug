// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public abstract class AudioPluginComponent : IAudioPluginComponent, IAudioConnectionPoint
{
    public AudioHostApplication? Host { get; private set; }

    protected IAudioConnectionPoint? ConnectionPoint { get; private set; }

    protected virtual void Terminate()
    {
    }

    protected virtual void OnMessage(AudioMessage message)
    {
    }

    protected virtual void OnConnect(IAudioConnectionPoint connectionPoint)
    {
    }

    protected virtual void OnDisconnect(IAudioConnectionPoint connectionPoint)
    {
    }

    bool IAudioPluginComponent.Initialize(AudioHostApplication hostApplication)
    {
        // Don't try to initialize if we have been initialized already (The host is active)
        if (Host == null && InitializeInternal(hostApplication))
        {
            Host = hostApplication;
            return true;
        }
        return false;
    }

    internal abstract bool InitializeInternal(AudioHostApplication hostApplication);

    internal virtual void TerminateInternal()
    {
        Host?.Dispose();
        Host = null;
    }

    void IAudioPluginComponent.Terminate()
    {
        try
        {
            Terminate();
        }
        finally
        {
            TerminateInternal();
        }
    }

    void IAudioConnectionPoint.Connect(IAudioConnectionPoint connectionPoint)
    {
        ConnectionPoint = connectionPoint;
        OnConnect(connectionPoint);
    }

    void IAudioConnectionPoint.Disconnect(IAudioConnectionPoint connectionPoint)
    {
        OnDisconnect(connectionPoint);
        ConnectionPoint = null;
    }

    void IAudioConnectionPoint.Notify(AudioMessage message)
    {
        OnMessage(message);
    }
}