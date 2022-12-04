// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using NPlug.Backend;
using NPlug.Vst3;

namespace NPlug;

public readonly struct AudioContextMenu
{
    private readonly IAudioContextMenuBackend _backend;
    internal readonly IntPtr NativeContext;

    public AudioContextMenu(IAudioContextMenuBackend backend, IntPtr nativeContext)
    {
        _backend = backend;
        NativeContext = nativeContext;
    }

    /// <summary>
    /// Gets the number of menu items.
    /// </summary>
    public int ItemCount => _backend?.GetItemCount(this) ?? 0;

    /// <summary>
    /// Gets a menu item and its target (target could be not assigned).
    /// </summary>
    public void GetItem(int index, out AudioContextMenuItem item, out AudioContextMenuAction? target)
    {
        GetSafeBackend().GetItem(this, index, out item, out target);
    }

    /// <summary>
    /// Adds a menu item and its target.
    /// </summary>
    public void AddItem(in AudioContextMenuItem item, AudioContextMenuAction target)
    {
        GetSafeBackend().AddItem(this, item, target);
    }

    /// <summary>
    /// Removes a menu item.
    /// </summary>
    public void RemoveItem(in AudioContextMenuItem item, AudioContextMenuAction target)
    {
        GetSafeBackend().RemoveItem(this, item, target);
    }

    /// <summary>
    /// Pop-ups the menu. Coordinates are relative to the top-left position of the plug-ins view.
    /// </summary>
    public void Popup(int x, int y)
    {
        GetSafeBackend().Popup(this, x, y);
    }

    private IAudioContextMenuBackend GetSafeBackend()
    {
        if (_backend is null) ThrowNotInitialized();
        return _backend;
    }

    [DoesNotReturn]
    private static void ThrowNotInitialized()
    {
        throw new InvalidOperationException("This context menu is not initialized");
    }
}