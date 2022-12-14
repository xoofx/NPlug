// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

public interface IAudioContextMenu : IDisposable
{
    /// <summary>
    /// Gets the number of menu items.
    /// </summary>
    int GetItemCount();

    /// <summary>
    /// Gets a menu item and its target (target could be not assigned).
    /// </summary>
    void GetItem(int index, out AudioContextMenuItem item, out AudioContextMenuAction? target);

    /// <summary>
    /// Adds a menu item and its target.
    /// </summary>
    void AddItem(in AudioContextMenuItem item, AudioContextMenuAction? target);

    /// <summary>
    /// Removes a menu item.
    /// </summary>
    void RemoveItem(in AudioContextMenuItem item, AudioContextMenuAction? target);

    /// <summary>
    /// Pop-ups the menu. Coordinates are relative to the top-left position of the plug-ins view.
    /// </summary>
    void Popup(int x, int y);
}