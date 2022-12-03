// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Backend;

public interface IAudioEventListBackend
{
    /// <summary>
    /// Returns the count of events.
    /// </summary>
    int GetEventCount(in AudioEventList eventList);

    /// <summary>
    /// Gets parameter by index.
    /// </summary>
    bool TryGetEvent(in AudioEventList eventList, int index, out AudioEvent evt);

    /// <summary>
    /// Adds a new event.
    /// </summary>
    bool TryAddEvent(in AudioEventList eventList, in AudioEvent evt);
}