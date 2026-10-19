// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NPlug.Interop;
using NUnit.Framework;

namespace NPlug.Tests;

/// <summary>
/// Drives <see cref="IAudioPluginView"/> through the native IPlugView vtable, as a host would.
/// </summary>
public unsafe class TestPluginViewInterop
{
    private static int s_resizeCallCount;
    private static int s_resizeResult;

    [Test]
    public void SetFrameNullIsForwardedAsNull()
    {
        var view = new RecordingView();
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(view);
        var plugView = comObject.QueryInterface<LibVst.IPlugView>();
        try
        {
            var frame = FakeFrame.Create();
            try
            {
                Assert.AreEqual(LibVst.ComResult.Ok, plugView->setFrame(frame).Value);
                Assert.IsNotNull(view.Frame);

                Assert.AreEqual(LibVst.ComResult.Ok, plugView->setFrame(null).Value);
                Assert.IsNull(view.Frame, "setFrame(nullptr) must reach the view as null, not as a wrapper around a null pointer.");
            }
            finally
            {
                FakeFrame.Free(frame);
            }
        }
        finally
        {
            comObject.ReleaseRef();
        }
    }

    [Test]
    public void ResizeViewReleasesItsReferenceAndReturnsTheHostResult()
    {
        var view = new RecordingView();
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(view);
        var plugView = comObject.QueryInterface<LibVst.IPlugView>();
        var frame = FakeFrame.Create();
        try
        {
            plugView->setFrame(frame);
            var refCountBefore = comObject.ReferenceCount;

            s_resizeCallCount = 0;
            s_resizeResult = LibVst.ComResult.Ok;
            Assert.IsTrue(view.Frame!.ResizeView(view, new ViewRectangle(0, 0, 300, 200)));
            Assert.AreEqual(1, s_resizeCallCount);
            Assert.AreEqual(refCountBefore, comObject.ReferenceCount, "ResizeView must not leak a reference on the view.");

            s_resizeResult = LibVst.ComResult.False;
            Assert.IsFalse(view.Frame!.ResizeView(view, new ViewRectangle(0, 0, 300, 200)), "A refused resize must be reported.");
            Assert.AreEqual(refCountBefore, comObject.ReferenceCount);

            plugView->setFrame(null);
        }
        finally
        {
            FakeFrame.Free(frame);
            comObject.ReleaseRef();
        }
    }

    [Test]
    public void KeyWheelAndScaleResultsComeFromTheView()
    {
        var view = new RecordingView();
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(view);
        var plugView = comObject.QueryInterface<LibVst.IPlugView>();
        var scaleSupport = comObject.QueryInterface<LibVst.IPlugViewContentScaleSupport>();
        try
        {
            view.Handles = false;
            Assert.AreEqual(LibVst.ComResult.False, plugView->onKeyDown(32, 0, 0).Value, "An unhandled key must return kResultFalse.");
            Assert.AreEqual(LibVst.ComResult.False, plugView->onKeyUp(32, 0, 0).Value);
            Assert.AreEqual(LibVst.ComResult.False, plugView->onWheel(1f).Value);
            Assert.AreEqual(LibVst.ComResult.False, scaleSupport->setContentScaleFactor(1.5f).Value);

            view.Handles = true;
            Assert.AreEqual(LibVst.ComResult.Ok, plugView->onKeyDown(32, 0, 0).Value);
            Assert.AreEqual(LibVst.ComResult.Ok, plugView->onKeyUp(32, 0, 0).Value);
            Assert.AreEqual(LibVst.ComResult.Ok, plugView->onWheel(1f).Value);
            Assert.AreEqual(LibVst.ComResult.Ok, scaleSupport->setContentScaleFactor(1.5f).Value);
            Assert.AreEqual(1.5f, view.LastScaleFactor);
        }
        finally
        {
            comObject.ReleaseRef();
            comObject.ReleaseRef();
        }
    }

    [Test]
    public void FinalReleaseKeepsAComObjectReferencedDuringDispose()
    {
        var view = new RecordingView();
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(view);
        comObject.QueryInterface<LibVst.IPlugView>();
        // Dispose takes a new reference on the COM object: it must stay alive instead of returning to the pool.
        view.OnDispose = () =>
        {
            view.OnDispose = null;
            comObject.AddRef();
        };
        Assert.DoesNotThrow(() => comObject.ReleaseRef());
        Assert.IsTrue(view.Disposed);
        Assert.AreEqual(1u, comObject.ReferenceCount);
        Assert.That(comObject.Target, Is.SameAs(view));
        Assert.That(LibVst.ComObjectManager.Instance.GetOrCreateComObject(view), Is.Not.SameAs(comObject), "a released object is no longer handed out");
        Assert.AreEqual(0u, comObject.ReleaseRef());
        Assert.IsNull(comObject.Target);
    }

    [Test]
    public void FinalReleaseToleratesComCallsOnTheTargetDuringDispose()
    {
        var view = new RecordingView();
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(view);
        comObject.QueryInterface<LibVst.IPlugView>();
        int disposeCount = 0;
        // Dispose queries the target again and releases it (a nested final release on a new COM object).
        view.OnDispose = () =>
        {
            if (++disposeCount > 1) return;
            var nested = LibVst.ComObjectManager.Instance.GetOrCreateComObject(view);
            Assert.That(nested, Is.Not.SameAs(comObject));
            nested.QueryInterface<LibVst.IPlugView>();
            nested.ReleaseRef();
        };
        Assert.DoesNotThrow(() => comObject.ReleaseRef());
        Assert.AreEqual(0u, comObject.ReferenceCount);
        Assert.IsFalse(LibVst.ComObjectManager.Instance.GetAliveComObjects().Any(x => ReferenceEquals(x.Target, view)));
    }

    /// <summary>
    /// Minimal native IPlugFrame implementation (vtable: queryInterface, addRef, release, resizeView).
    /// </summary>
    private static class FakeFrame
    {
        public static LibVst.IPlugFrame* Create()
        {
            var frame = (LibVst.IPlugFrame*)NativeMemory.AllocZeroed((nuint)sizeof(LibVst.IPlugFrame));
            var vtbl = (void**)NativeMemory.AllocZeroed((nuint)(sizeof(void*) * 4));
            vtbl[0] = (delegate* unmanaged[MemberFunction]<LibVst.IPlugFrame*, Guid*, void**, int>)&QueryInterface;
            vtbl[1] = (delegate* unmanaged[MemberFunction]<LibVst.IPlugFrame*, uint>)&AddRef;
            vtbl[2] = (delegate* unmanaged[MemberFunction]<LibVst.IPlugFrame*, uint>)&Release;
            vtbl[3] = (delegate* unmanaged[MemberFunction]<LibVst.IPlugFrame*, LibVst.IPlugView*, LibVst.ViewRect*, int>)&ResizeView;
            frame->Vtbl = vtbl;
            return frame;
        }

        public static void Free(LibVst.IPlugFrame* frame)
        {
            NativeMemory.Free(frame->Vtbl);
            NativeMemory.Free(frame);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        private static int QueryInterface(LibVst.IPlugFrame* self, Guid* iid, void** obj)
        {
            *obj = null;
            return unchecked((int)0x80004002); // kNoInterface
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        private static uint AddRef(LibVst.IPlugFrame* self) => 1;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        private static uint Release(LibVst.IPlugFrame* self) => 1;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        private static int ResizeView(LibVst.IPlugFrame* self, LibVst.IPlugView* view, LibVst.ViewRect* newSize)
        {
            s_resizeCallCount++;
            return s_resizeResult;
        }
    }

    private sealed class RecordingView : IAudioPluginView, IDisposable
    {
        public IAudioPluginFrame? Frame { get; private set; }
        public bool Handles { get; set; }
        public float LastScaleFactor { get; private set; }
        public bool Disposed { get; private set; }
        public Action? OnDispose { get; set; }

        public bool IsPlatformTypeSupported(AudioPluginViewPlatform platform) => true;
        public void Attached(nint parent, AudioPluginViewPlatform type) { }
        public void Removed() { }
        public bool OnWheel(float distance) => Handles;
        public bool OnKeyDown(ushort key, short keyCode, short modifiers) => Handles;
        public bool OnKeyUp(ushort key, short keyCode, short modifiers) => Handles;
        public ViewRectangle Size => new(0, 0, 100, 100);
        public void OnSize(ViewRectangle newSize) { }
        public void OnFocus(bool state) { }
        public void SetFrame(IAudioPluginFrame? frame) => Frame = frame;
        public bool CanResize() => false;
        public bool CheckSizeConstraint(ref ViewRectangle rect) => true;

        public bool SetContentScaleFactor(float factor)
        {
            LastScaleFactor = factor;
            return Handles;
        }

        public bool TryFindParameter(int xPos, int yPos, out AudioParameterId parameterId)
        {
            parameterId = default;
            return false;
        }

        public void Dispose()
        {
            Disposed = true;
            OnDispose?.Invoke();
        }
    }
}
