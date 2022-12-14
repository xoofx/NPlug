// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// A plugin class info
/// </summary>
public abstract class AudioPluginClassInfo
{
    private static readonly Version EmptyVersion = new (0, 0);

    /// <summary>
    /// Creates a new instance of this plugin info.
    /// </summary>
    /// <param name="classId"></param>
    /// <param name="name"></param>
    /// <param name="category"></param>
    internal AudioPluginClassInfo(Guid classId, string name, AudioPluginCategory pluginCategory)
    {
        ClassId = classId;
        Name = name;
        ClassFlags = AudioPluginFlags.None;
        Cardinality = int.MaxValue;
        Vendor = string.Empty;
        Version = EmptyVersion;
        PluginCategory = pluginCategory;
    }

    /// <summary>
    /// see @ref PClassInfo
    /// </summary>
    public Guid ClassId { get; }

    /// <summary>
    /// Gets or init the category.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Gets the category of the plugin
    /// </summary>
    public AudioPluginCategory PluginCategory { get; }

    /// <summary>
    /// flags used for a specific category, must be defined where category is defined
    /// </summary>
    public AudioPluginFlags ClassFlags { get; init; }

    /// <summary>
    /// see @ref PClassInfo
    /// </summary>
    public int Cardinality { get; init; }

    /// <summary>
    /// overwrite vendor information from factory info
    /// </summary>
    public string Vendor { get; init; }

    /// <summary>
    /// Version string (e.g. "1.0.0.512" with Major.Minor.Subversion.Build)
    /// </summary>
    public Version Version { get; init; }
}