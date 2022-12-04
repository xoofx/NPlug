// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// A context menu item.
/// </summary>
/// <param name="Name">Name of the item.</param>
/// <param name="Tag">Identifier tag of the item.</param>
/// <param name="Flags">Flags of the item</param>
public readonly record struct AudioContextMenuItem(string Name, int Tag = 0, AudioContextMenuItemFlags Flags = AudioContextMenuItemFlags.None);