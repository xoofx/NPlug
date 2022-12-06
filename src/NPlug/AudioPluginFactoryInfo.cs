// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public sealed class AudioPluginFactoryInfo
{
    public AudioPluginFactoryInfo()
    {
        Vendor = string.Empty;
        Url = string.Empty;
        Email = string.Empty;
        Flags = AudioPluginFactoryFlags.NoFlags;
    }

    public AudioPluginFactoryInfo(string vendor, string url, string email, AudioPluginFactoryFlags flags = AudioPluginFactoryFlags.NoFlags)
    {
        Vendor = vendor;
        Url = url;
        Email = email;
        Flags = flags;
    }
    
    public string Vendor { get; init; }

    public string Url { get; init; }

    public string Email { get; init; }

    public AudioPluginFactoryFlags Flags { get; init; }
}