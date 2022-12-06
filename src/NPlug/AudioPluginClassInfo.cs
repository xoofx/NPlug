// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// A plugin class info
/// </summary>
public sealed class AudioPluginClassInfo
{
    private static readonly Version EmptyVersion = new (0, 0);

    /// <summary>
    /// Creates a new instance of this plugin info.
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="name"></param>
    public AudioPluginClassInfo(Guid guid, string name)
    {
        Id = guid;
        Name = name;
        Category = string.Empty;
        Name = name;
        ClassFlags = AudioPluginFlags.None;
        SubCategories = string.Empty;
        Vendor = string.Empty;
        Cardinality = -1;
        Version = EmptyVersion;
    }

    /// <summary>
    /// see @ref PClassInfo
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// see @ref PClassInfo
    /// </summary>
    public int Cardinality { get; init; }

    /// <summary>
    /// Gets or init the category.
    /// </summary>
    public string Category { get; init; }

    /// <summary>
    /// Gets or init the category.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// flags used for a specific category, must be defined where category is defined
    /// </summary>
    public AudioPluginFlags ClassFlags { get; init; }

    /// <summary>
    /// module specific subcategories, can be more than one, logically added by the OR operator
    /// </summary>
    public string SubCategories { get; init; }

    /// <summary>
    /// overwrite vendor information from factory info
    /// </summary>
    public string Vendor { get; init; }

    /// <summary>
    /// Version string (e.g. "1.0.0.512" with Major.Minor.Subversion.Build)
    /// </summary>
    public Version Version { get; init; }
}