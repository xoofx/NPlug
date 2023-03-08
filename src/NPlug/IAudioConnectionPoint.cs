// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Connect a component with another one.
/// </summary>
public interface IAudioConnectionPoint
{
    /// <summary>
    /// Connects this instance with another connection point.
    /// </summary>
    /// <param name="connectionPoint">The other connection point.</param>
    void Connect(IAudioConnectionPoint connectionPoint);

    /// <summary>
    /// Disconnects a given connection point from this.
    /// </summary>
    /// <param name="connectionPoint">The other connection point.</param>
    void Disconnect(IAudioConnectionPoint connectionPoint);

    /// <summary>
    /// Called when a message has been sent from the connection point to this.
    /// </summary>
    /// <param name="message">The message notified.</param>
    void Notify(AudioMessage message);
}