// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public enum AudioPluginViewPlatform
{
    /// <summary>
    /// The parent parameter in <see cref="IAudioPluginView.Attached"/>::attached() is a HWND handle. You should attach a child window to it.
    /// (Microsoft Windows)
    /// </summary>
    Hwnd,

    /// <summary>
    /// The parent parameter in IPlugView::attached() is a WindowRef. You should attach a HIViewRef to the content view of the window.
    /// HIViewRef. (Mac OS X)
    /// </summary>
    HIView,

    /// <summary>
    /// The parent parameter in IPlugView::attached() is a NSView pointer. You should attach a HIViewRef to the content view of the window.
    /// NSView pointer. (Mac OS X)
    /// </summary>
    NSView,

    /// <summary>
    /// The parent parameter in IPlugView::attached() is a UIView pointer. You should attach an UIView to it.
    /// UIView pointer. (iOS)
    /// </summary>
    UIView,

    /// <summary>
    /// The parent parameter in IPlugView::attached() is a X11 Window supporting XEmbed. You should attach a Window to it that supports the XEmbed extension.
    /// X11 Window ID. (X11)
    /// See https://standards.freedesktop.org/xembed-spec/xembed-spec-latest.html
    /// </summary>
    X11EmbedWindowID
}