// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Defines the information of a plugin factory.
/// </summary>
public sealed class AudioPluginFactoryInfo
{
    /// <summary>
    /// Creates a new instance of this plugin factory info.
    /// </summary>
    public AudioPluginFactoryInfo()
    {
        Vendor = string.Empty;
        Url = string.Empty;
        Email = string.Empty;
        Flags = AudioPluginFactoryFlags.NoFlags;
    }

    /// <summary>
    /// Creates a new instance of this plugin factory info.
    /// </summary>
    public AudioPluginFactoryInfo(string vendor, string url, string email, AudioPluginFactoryFlags flags = AudioPluginFactoryFlags.NoFlags)
    {
        Vendor = vendor;
        Url = url;
        Email = email;
        Flags = flags;
    }

    /// <summary>
    /// Gets the vendor information.
    /// </summary>
    public string Vendor { get; init; }

    /// <summary>
    /// Gets the url information.
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// Gets the email information.
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// Gets the flags of this plugin factory.
    /// </summary>
    public AudioPluginFactoryFlags Flags { get; init; }
}