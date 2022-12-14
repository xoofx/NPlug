// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.Interop;
using System.Runtime.InteropServices;
using static NPlug.Interop.LibVst;

namespace NPlug;

/// <summary>
/// Plug-in definition of a view.
/// </summary>
/// <remarks>
///  pluginGUI vstIPlug vst300
/// - [plug imp]
/// - [released: 3.0.0]
/// </remarks>
/// <seealso cref="platformUITypeIPlugFrame, "/>
/// <par>
/// Sizing of a view
/// Usually, the size of a plug-in view is fixed. But both the host and the plug-in can cause
/// a view to be resized:@n - @b Host: If IPlugView::canResize () returns kResultTrue the host will set up the window
/// so that the user can resize it. While the user resizes the window,
/// IPlugView::checkSizeConstraint () is called, allowing the plug-in to change the size to a valid
/// a valid supported rectangle size. The host then resizes the window to this rect and has to call IPlugView::onSize ().@n @n - @b Plug-in: The plug-in can call IPlugFrame::resizeView () and cause the host to resize the
/// window.@n @n Afterwards, in the same callstack, the host has to call IPlugView::onSize () if a resize is needed (size was changed).
/// Note that if the host calls IPlugView::getSize () before calling IPlugView::onSize () (if needed),
/// it will get the current (old) size not the wanted one!!@n Here the calling sequence:@n - plug-in-&gt;host: IPlugFrame::resizeView (newSize)
/// - host-&gt;plug-in (optional): IPlugView::getSize () returns the currentSize (not the newSize!)
/// - host-&gt;plug-in: if newSize is different from the current size: IPlugView::onSize (newSize)
/// - host-&gt;plug-in (optional): IPlugView::getSize () returns the newSize@n &lt;b&gt;Please only resize the platform representation of the view when IPlugView::onSize () is
/// called.&lt;/b&gt;
/// </par>
/// <par>
/// Keyboard handling
/// The plug-in view receives keyboard events from the host. A view implementation must not handle
/// keyboard events by the means of platform callbacks, but let the host pass them to the view. The host
/// depends on a proper return value when IPlugView::onKeyDown is called, otherwise the plug-in view may
/// cause a malfunction of the host's key command handling.
/// </par>
public interface IAudioPluginView
{
    /// <summary>
    /// Is Platform UI Type supported
    /// </summary>
    /// <param name="platform">the type of the platform</param>
    bool IsPlatformTypeSupported(OSPlatform platform);

    /// <summary>
    /// The parent window of the view has been created, the (platform) representation of the view
    /// should now be created as well.
    /// Note that the parent is owned by the caller and you are not allowed to alter it in any way
    /// other than adding your own views.
    /// Note that in this call the plug-in could call a IPlugFrame::resizeView ()!
    /// </summary>
    /// <param name="parent">: platform handle of the parent window or view</param>
    /// <param name="type">: @ref platformUIType which should be created</param>
    void Attached(nint parent, PlatformViewType type);

    /// <summary>
    /// The parent window of the view is about to be destroyed.
    /// You have to remove all your own views from the parent window or view.
    /// </summary>
    void Removed();

    /// <summary>
    /// Handling of mouse wheel.
    /// </summary>
    void OnWheel(float distance);
    
    /// <summary>
    /// Handling of keyboard events : Key Down.
    /// </summary>
    /// <param name="key">: unicode code of key</param>
    /// <param name="keyCode">: virtual keycode for non ascii keys - see @ref VirtualKeyCodes in keycodes.h</param>
    /// <param name="modifiers">: any combination of modifiers - see @ref KeyModifier in keycodes.h</param>
    /// <returns>kResultTrue if the key is handled, otherwise kResultFalse. @n &lt;b&gt;Please note that kResultTrue must only be returned if the key has really been
    /// handled. &lt;/b&gt;Otherwise key command handling of the host might be blocked!</returns>
    void OnKeyDown(ushort key, short keyCode, short modifiers);

    /// <summary>
    /// Handling of keyboard events : Key Up.
    /// </summary>
    /// <param name="key">: unicode code of key</param>
    /// <param name="keyCode">: virtual keycode for non ascii keys - see @ref VirtualKeyCodes in keycodes.h</param>
    /// <param name="modifiers">: any combination of KeyModifier - see @ref KeyModifier in keycodes.h</param>
    /// <returns>kResultTrue if the key is handled, otherwise return kResultFalse.</returns>
    void OnKeyUp(ushort key, short keyCode, short modifiers);

    /// <summary>
    /// Returns the size of the platform representation of the view.
    /// </summary>
    ViewRectangle Size { get; }

    /// <summary>
    /// Resizes the platform representation of the view to the given rect. Note that if the plug-in
    /// requests a resize (IPlugFrame::resizeView ()) onSize has to be called afterward.
    /// </summary>
    void OnSize(ViewRectangle newSize);
    
    /// <summary>
    /// Focus changed message.
    /// </summary>
    void OnFocus(bool state);

    /// <summary>
    /// Sets IPlugFrame object to allow the plug-in to inform the host about resizing.
    /// </summary>
    void SetFrame(IAudioPluginFrame frame);

    /// <summary>
    /// Is view sizable by user.
    /// </summary>
    bool CanResize();

    /// <summary>
    /// On live resize this is called to check if the view can be resized to the given rect, if not
    /// adjust the rect to the allowed size.
    /// </summary>
    bool CheckSizeConstraint(ViewRectangle rect);

    /// <summary>
    /// This interface communicates the content scale factor from the host to the plug-in view on
    /// systems where plug-ins cannot get this information directly like Microsoft Windows.
    /// 
    /// The host calls setContentScaleFactor directly before or after the plug-in view is attached and when
    /// the scale factor changes while the view is attached (system change or window moved to another screen
    /// with different scaling settings).
    /// 
    /// The host may call setContentScaleFactor in a different context, for example: scaling the plug-in
    /// editor for better readability.
    /// 
    /// When a plug-in handles this (by returning kResultTrue), it needs to scale the width and height of
    /// its view by the scale factor and inform the host via a IPlugFrame::resizeView(). The host will then
    /// call IPlugView::onSize().
    /// 
    /// Note that the host is allowed to call setContentScaleFactor() at any time the IPlugView is valid.
    /// If this happens before the IPlugFrame object is set on your view, make sure that when the host calls
    /// IPlugView::getSize() afterwards you return the size of your view for that new scale factor.
    /// 
    /// It is recommended to implement this interface on Microsoft Windows to let the host know that the
    /// plug-in is able to render in different scalings.
    /// </summary>
    /// <param name="factor"></param>
    void SetContentScaleFactor(float factor);
}

public interface IAudioPluginFrame
{
    void ResizeView(IAudioPluginView view, ViewRectangle newSize);
}


public readonly record struct ViewRectangle(int Left, int Top, int Right, int Bottom);


public enum PlatformViewType
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