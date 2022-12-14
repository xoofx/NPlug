// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// An <see cref="AudioController"/> plugin class info
/// </summary>
public sealed class AudioTestProviderClassInfo : AudioPluginClassInfo
{
    /// <summary>
    /// Creates a new instance of this plugin info.
    /// </summary>
    /// <param name="classId"></param>
    /// <param name="name"></param>
    public AudioTestProviderClassInfo(Guid classId, string name) : base(classId, name, AudioPluginCategory.TestProvider)
    {
    }
}