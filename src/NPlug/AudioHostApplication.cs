// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

public abstract class AudioHostApplication : IDisposable
{
    internal AudioHostApplication(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public abstract bool TryCreateMessage(string messageId, out AudioMessage message);

    public abstract void Dispose();
}