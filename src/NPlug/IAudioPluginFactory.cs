// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// A factory to create <see cref="IAudioPluginObject"/> instances.
/// </summary>
public interface IAudioPluginFactory
{
    /// <summary>
    /// Gets information about this factory.
    /// </summary>
    AudioPluginFactoryInfo FactoryInfo { get; }

    /// <summary>
    /// Gets the number of plugins this factory supports.
    /// </summary>
    int PluginClassInfoCount { get; }

    /// <summary>
    /// Gets the information about a plugin class at the specified index.
    /// </summary>
    /// <param name="index">The index of the plugin.</param>
    AudioPluginClassInfo GetPluginClassInfo(int index);

    /// <summary>
    /// Creates a new instance of the plugin with the specified id.
    /// </summary>
    /// <param name="pluginId">The id of the plugin.</param>
    /// <returns>A new instance of the plugin; otherwise null if this factory does not support this id.</returns>
    IAudioPluginObject? CreateInstance(Guid pluginId);
}