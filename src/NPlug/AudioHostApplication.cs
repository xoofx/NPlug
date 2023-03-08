// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// Base class for a host application.
/// </summary>
public abstract class AudioHostApplication : IDisposable
{
    internal AudioHostApplication(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name of the host.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Tries to create an audio message. The output message must be disposed after using it.
    /// </summary>
    /// <param name="messageId">An id for the message.</param>
    /// <param name="message">The output message.</param>
    /// <returns><c>true</c> if the message was successfully created.</returns>
    public abstract bool TryCreateMessage(string messageId, out AudioMessage message);

    /// <summary>
    /// Dispose this host application.
    /// </summary>
    public abstract void Dispose();
}