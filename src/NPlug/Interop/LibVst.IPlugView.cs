// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IPlugView
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioPluginView Get(IPlugView* self) => ((ComObjectHandle*)self)->As<IAudioPluginView>();

        private static partial ComResult isPlatformTypeSupported_ToManaged(IPlugView* self, FIDString type)
        {
            return TryGetPlatform(type, out var platform) && Get(self).IsPlatformTypeSupported(platform);
        }

        private static partial ComResult attached_ToManaged(IPlugView* self, void* parent, FIDString type)
        {
            if (TryGetPlatform(type, out var platform))
            {
                Get(self).Attached((nint)parent, platform);
                return true;
            }

            return false;
        }

        private static partial ComResult removed_ToManaged(IPlugView* self)
        {
            Get(self).Removed();
            return true;
        }

        private static partial ComResult onWheel_ToManaged(IPlugView* self, float distance)
        {
            Get(self).OnWheel(distance);
            return true;
        }

        private static partial ComResult onKeyDown_ToManaged(IPlugView* self, ushort key, short keyCode, short modifiers)
        {
            Get(self).OnKeyDown(key, keyCode, modifiers);
            return true;
        }

        private static partial ComResult onKeyUp_ToManaged(IPlugView* self, ushort key, short keyCode, short modifiers)
        {
            Get(self).OnKeyUp(key, keyCode, modifiers);
            return true;
        }

        private static partial ComResult getSize_ToManaged(IPlugView* self, ViewRect* size)
        {
            *((ViewRectangle*)size) = Get(self).Size;
            return true;
        }

        private static partial ComResult onSize_ToManaged(IPlugView* self, ViewRect* newSize)
        {
            Get(self).OnSize(*((ViewRectangle*)newSize));
            return true;
        }

        private static partial ComResult onFocus_ToManaged(IPlugView* self, byte state)
        {
            Get(self).OnFocus(state != 0);
            return true;
        }

        private static partial ComResult setFrame_ToManaged(IPlugView* self, IPlugFrame* frame)
        {
            Get(self).SetFrame(new AudioPluginFrameVst(frame));
            return true;
        }

        private static partial ComResult canResize_ToManaged(IPlugView* self)
        {
            return Get(self).CanResize();
        }

        private static partial ComResult checkSizeConstraint_ToManaged(IPlugView* self, ViewRect* rect)
        {
            return Get(self).CheckSizeConstraint(ref *(ViewRectangle*)rect);
        }

        private static bool TryGetPlatform(FIDString type, out AudioPluginViewPlatform platform)
        {
            var span = new ReadOnlySpan<byte>(type.Value, int.MaxValue);
            span = span.Slice(0, span.IndexOf((byte)0));
            platform = default;
            if (span.SequenceEqual("HWND"u8))
            {
                platform = AudioPluginViewPlatform.Hwnd;
            }
            else if (span.SequenceEqual("HIView"u8))
            {
                platform = AudioPluginViewPlatform.HIView;
            }
            else if (span.SequenceEqual("UIView"u8))
            {
                platform = AudioPluginViewPlatform.UIView;
            }
            else if (span.SequenceEqual("X11EmbedWindowID"u8))
            {
                platform = AudioPluginViewPlatform.X11EmbedWindowID;
            }
            else
            {
                return false;
            }

            return true;
        }

        private class AudioPluginFrameVst : IAudioPluginFrame
        {
            private readonly IPlugFrame* _frame;

            public AudioPluginFrameVst(IPlugFrame* frame)
            {
                _frame = frame;
            }

            public void ResizeView(IAudioPluginView view, ViewRectangle newSize)
            {
                var comObject = ComObjectManager.Instance.GetOrCreateComObject(view);
                var plugView = comObject.QueryInterface<IPlugView>();
                _frame->resizeView(plugView, (ViewRect*)&newSize);
            }
        }
    }
}