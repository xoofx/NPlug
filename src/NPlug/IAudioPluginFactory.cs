// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

public interface IAudioPluginFactory
{
    AudioPluginFactoryInfo FactoryInfo { get; }

    int PluginClassInfoCount { get; }

    AudioPluginClassInfo GetPluginClassInfo(int index);

    IAudioPluginComponent? CreateInstance(Guid pluginId);
}