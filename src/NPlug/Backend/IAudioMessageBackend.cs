// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug.Backend;

/// <summary>
/// Host backend for manipulating message.
/// </summary>
public interface IAudioMessageBackend
{
    /// <summary>
    /// Gets the id of the specified message.
    /// </summary>
    string GetId(in AudioMessage message);
    /// <summary>
    /// Sets the id of the specified message.
    /// </summary>
    void SetId(in AudioMessage message, string id);
    /// <summary>
    /// Destroys the specified message.
    /// </summary>
    void Destroy(in AudioMessage message);
}