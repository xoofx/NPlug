// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Base class for an audio plugin component.
/// </summary>
public abstract class AudioPluginComponent : IAudioPluginComponent, IAudioConnectionPoint
{
    /// <inheritdoc />
    public AudioHostApplication? Host { get; private set; }

    /// <summary>
    /// Gets the connection point associated to this component. The connection point is associated when this component is connected to another component.
    /// </summary>
    protected IAudioConnectionPoint? ConnectionPoint { get; private set; }

    /// <summary>
    /// Called when this component is terminated.
    /// </summary>
    protected virtual void Terminate()
    {
    }

    /// <summary>
    /// Called when this component receives a message from another component.
    /// </summary>
    /// <param name="message"></param>
    protected virtual void OnMessage(AudioMessage message)
    {
    }

    /// <summary>
    /// Called when this component is connected to another component.
    /// </summary>
    /// <param name="connectionPoint"></param>
    protected virtual void OnConnect(IAudioConnectionPoint connectionPoint)
    {
    }

    /// <summary>
    /// Called when this component is disconnected from another component.
    /// </summary>
    /// <param name="connectionPoint"></param>
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