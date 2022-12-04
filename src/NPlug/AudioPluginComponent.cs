// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public abstract class AudioPluginComponent : IAudioPluginComponent, IAudioConnectionPoint
{
    private IAudioConnectionPoint? _connectionPoint;

    public AudioHostApplication? Host { get; private set; }
    
    protected virtual void Terminate()
    {
    }

    protected virtual void OnMessage(AudioMessage message)
    {
    }

    bool IAudioPluginComponent.Initialize(AudioHostApplication hostApplication)
    {
        if (InitializeInternal(hostApplication))
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
        _connectionPoint = connectionPoint;
    }

    void IAudioConnectionPoint.Disconnect(IAudioConnectionPoint connectionPoint)
    {
        _connectionPoint = null;
    }

    void IAudioConnectionPoint.Notify(AudioMessage message)
    {
        OnMessage(message);
    }
}