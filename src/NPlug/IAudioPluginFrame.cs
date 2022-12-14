// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Callback interface passed to IPlugView.
/// \ingroup pluginGUI vstIHost vst300
/// - [host imp]
/// - [released: 3.0.0]
/// - [mandatory]
///
/// Enables a plug-in to resize the view and cause the host to resize the window.
/// </summary>
public interface IAudioPluginFrame
{
    /// <summary>
    /// Called to inform the host about the resize of a given view. Afterwards the host has to call <see cref="IAudioPluginView.OnSize"/>.
    /// </summary>
    /// <param name="view"></param>
    /// <param name="newSize"></param>
    void ResizeView(IAudioPluginView view, ViewRectangle newSize);
}