// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug.Backend;

public interface IAudioContextMenuBackend
{
    /// <summary>
    /// Gets the number of menu items.
    /// </summary>
    int GetItemCount(in AudioContextMenu contextMenu);

    /// <summary>
    /// Gets a menu item and its target (target could be not assigned).
    /// </summary>
    void GetItem(in AudioContextMenu contextMenu, int index, out AudioContextMenuItem item, out AudioContextMenuAction? target);

    /// <summary>
    /// Adds a menu item and its target.
    /// </summary>
    void AddItem(in AudioContextMenu contextMenu, in AudioContextMenuItem item, AudioContextMenuAction target);

    /// <summary>
    /// Removes a menu item.
    /// </summary>
    void RemoveItem(in AudioContextMenu contextMenu, in AudioContextMenuItem item, AudioContextMenuAction target);

    /// <summary>
    /// Pop-ups the menu. Coordinates are relative to the top-left position of the plug-ins view.
    /// </summary>
    void Popup(in AudioContextMenu contextMenu, int x, int y);
}