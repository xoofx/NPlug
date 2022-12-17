// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace NPlug;

/// <summary>
/// Export an <see cref="AudioPluginFactory"/> using a <see cref="ModuleInitializerAttribute"/>.
/// </summary>
public static class AudioPluginFactoryExporter
{
    /// <summary>
    /// Sets an <see cref="AudioPluginFactory"/> to export to the plugin host.
    /// </summary>
    /// <remarks>
    /// This property must be set from a static method having the <see cref="ModuleInitializerAttribute"/> attribute on it.
    /// </remarks>
    public static AudioPluginFactory? Instance { get; set; }
}