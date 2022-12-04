// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Delegate used by <see cref="AudioContextMenu"/>.
/// </summary>
/// <param name="tag">The associated tag to the triggered menu item (<see cref="AudioContextMenuItem.Tag"/>) </param>
public delegate void AudioContextMenuAction(uint tag);