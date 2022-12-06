// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

[Flags]
public enum AudioContextMenuItemFlags
{
    /// <summary>
    /// No options.
    /// </summary>
    None = 0,

    /// <summary>
    /// Item is a separator
    /// </summary>
    IsSeparator = 1 << 0,

    /// <summary>
    /// Item is disabled
    /// </summary>
    IsDisabled = 1 << 1,

    /// <summary>
    /// Item is checked
    /// </summary>
    IsChecked = 1 << 2,

    /// <summary>
    /// Item is a group start (like sub folder)
    /// </summary>
    IsGroupStart = 1 << 3 | IsDisabled,

    /// <summary>
    /// Item is a group end
    /// </summary>
    IsGroupEnd = 1 << 4 | IsSeparator,
}