// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

/// <summary>
/// An <see cref="AudioProcessor"/> plugin class info
/// </summary>
public sealed class AudioProcessorClassInfo : AudioPluginClassInfo
{
    /// <summary>
    /// Creates a new instance of this plugin info.
    /// </summary>
    /// <param name="classId"></param>
    /// <param name="name"></param>
    /// <param name="category"></param>
    public AudioProcessorClassInfo(Guid classId, string name, AudioProcessorCategory category) : base(classId, name, AudioPluginCategory.Processor)
    {
        Category = category;
    }

    /// <summary>
    /// Gets or init the category.
    /// </summary>
    public AudioProcessorCategory Category { get; init; }
}