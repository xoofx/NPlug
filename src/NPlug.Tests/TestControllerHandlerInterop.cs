// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NPlug.Interop;
using NUnit.Framework;

namespace NPlug.Tests;

/// <summary>
/// Drives controller callbacks through native vtables with actual, per-interface COM reference counts.
/// </summary>
public unsafe class TestControllerHandlerInterop
{
    [TestCase(0)]
    [TestCase(0b10101)]
    [TestCase(0b11111)]
    public void ReplacementAndDisconnectionBalanceAllInterfaces(int supportedInterfaces)
    {
        using var first = new NativeHandler(supportedInterfaces);
        using var second = new NativeHandler(supportedInterfaces);
        var controller = new Controller();
        using var export = new ControllerExport(controller);
        var ownedReferenceCount = 1 + BitOperations.PopCount((uint)supportedInterfaces);

        Assert.AreEqual(LibVst.ComResult.Ok, export.Native->setComponentHandler(first.Handler).Value);
        Assert.AreEqual(1 + ownedReferenceCount, first.ReferenceCount);
        var previous = controller.Handler!;
        Assert.AreEqual((supportedInterfaces & 1) != 0, previous.IsAdvancedEditSupported);
        Assert.AreEqual((supportedInterfaces & 2) != 0, previous.IsCreateContextMenuSupported);
        Assert.AreEqual((supportedInterfaces & 4) != 0, previous.IsRequestBusActivationSupported);
        Assert.AreEqual((supportedInterfaces & 8) != 0, previous.IsProgressSupported);
        Assert.AreEqual((supportedInterfaces & 16) != 0, previous.IsUnitAndProgramListSupported);

        Assert.AreEqual(LibVst.ComResult.Ok, export.Native->setComponentHandler(second.Handler).Value);
        first.AssertBalanced();
        Assert.AreEqual(1 + ownedReferenceCount, second.ReferenceCount);
        Assert.Throws<ObjectDisposedException>(() => previous.BeginEdit(1));
        Assert.Throws<ObjectDisposedException>(() => previous.StartGroupEdit());

        Assert.AreEqual(LibVst.ComResult.Ok, export.Native->setComponentHandler(null).Value);
        Assert.IsNull(controller.Handler);
        second.AssertBalanced();
        Assert.AreEqual(LibVst.ComResult.Ok, export.Native->setComponentHandler(null).Value);
        second.AssertBalanced();
    }

    [Test]
    public void ReinstallingTheSameHandlerNeverDropsItsLastReference()
    {
        using var host = new NativeHandler(0b11111);
        var controller = new Controller();
        using var export = new ControllerExport(controller);
        export.Native->setComponentHandler(host.Handler);
        host.ReleaseHostReference();
        Assert.AreEqual(6, host.ReferenceCount, "Only the references owned by the controller remain.");

        Assert.AreEqual(LibVst.ComResult.Ok, export.Native->setComponentHandler(host.Handler).Value);
        Assert.AreEqual(6, host.ReferenceCount);
        Assert.IsFalse(host.State->UsedAfterFinalRelease);
        controller.Handler!.BeginEdit(1);
        Assert.AreEqual(1, host.State->EditCalls);

        export.Native->setComponentHandler(null);
        host.AssertBalanced();
    }

    [TestCase(false)]
    [TestCase(true)]
    public void TerminationReleasesTheHandlerEvenWhenTheOverrideThrows(bool throwOnTerminate)
    {
        using var host = new NativeHandler(0b11111);
        var controller = new Controller { ThrowOnTerminate = throwOnTerminate };
        using var export = new ControllerExport(controller);
        export.Native->setComponentHandler(host.Handler);

        Assert.AreEqual(throwOnTerminate ? LibVst.ComResult.InternalError : LibVst.ComResult.Ok,
            ((LibVst.IPluginBase*)export.Native)->terminate().Value);
        Assert.IsTrue(controller.HadHandlerDuringTermination);
        Assert.IsNull(controller.Handler);
        host.AssertBalanced();
        ((LibVst.IPluginBase*)export.Native)->terminate();
        host.AssertBalanced();
    }

    [TestCase(false)]
    [TestCase(true)]
    public void FinalReleaseCleansUpCustomControllersWithoutTermination(bool throwOnDispose)
    {
        using var host = new NativeHandler(0b11111);
        var controller = new CustomController { ThrowOnDispose = throwOnDispose };
        using var export = new ControllerExport(controller);
        export.Native->setComponentHandler(host.Handler);
        var previous = controller.Handler!;

        if (throwOnDispose)
        {
            Assert.Throws<InvalidOperationException>(() => export.Dispose());
        }
        else
        {
            export.Dispose();
        }

        Assert.IsTrue(controller.HadHandlerDuringDisposal);
        Assert.IsNull(controller.Handler);
        host.AssertBalanced();
        Assert.Throws<ObjectDisposedException>(() => previous.RestartComponent(0));

        Assert.IsNull(export.ComObject.Target);
    }

    [Test]
    public void CollectingADetachedHandlerDoesNotReleaseNativeReferences()
    {
        using var host = new NativeHandler(0b11111);
        var controller = new Controller();
        using var export = new ControllerExport(controller);
        var weakHandler = AttachAndDetach(controller, export, host);
        host.AssertBalanced();
        var releaseCount = host.State->ReleaseCalls;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.IsFalse(weakHandler.IsAlive);
        Assert.AreEqual(releaseCount, host.State->ReleaseCalls, "No native releases may be deferred to a finalizer.");
        host.AssertBalanced();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference AttachAndDetach(Controller controller, ControllerExport export, NativeHandler host)
    {
        export.Native->setComponentHandler(host.Handler);
        var weakHandler = new WeakReference(controller.Handler);
        export.Native->setComponentHandler(null);
        return weakHandler;
    }

    [Test]
    public void FinalReleaseCleansUpAControllerThatDoesNotImplementDisposable()
    {
        using var host = new NativeHandler(0b11111);
        var controller = new Controller();
        using var export = new ControllerExport(controller);
        export.Native->setComponentHandler(host.Handler);

        export.Dispose();

        Assert.IsNull(controller.Handler);
        host.AssertBalanced();
    }

    [Test]
    public void ARejectedHandlerReleasesTheNewReferencesAndPreservesThePreviousHandler()
    {
        using var first = new NativeHandler(0b11111);
        using var rejected = new NativeHandler(0b11111);
        var controller = new CustomController();
        using var export = new ControllerExport(controller);
        export.Native->setComponentHandler(first.Handler);
        var previous = controller.Handler;
        controller.RejectHandler = true;

        Assert.AreEqual(LibVst.ComResult.InternalError, export.Native->setComponentHandler(rejected.Handler).Value);
        rejected.AssertBalanced();
        Assert.That(controller.Handler, Is.SameAs(previous));
        Assert.AreEqual(7, first.ReferenceCount);
        Assert.AreEqual(LibVst.ComResult.InternalError, export.Native->setComponentHandler(null).Value);
        Assert.AreEqual(7, first.ReferenceCount);

        controller.RejectHandler = false;
        export.Native->setComponentHandler(null);
        first.AssertBalanced();
    }

    [TestCase(false)]
    [TestCase(true)]
    public void AReentrantReplacementKeepsOnlyTheLastHandler(bool throwAfterReentry)
    {
        using var first = new NativeHandler(0b11111);
        using var intermediate = new NativeHandler(0b11111);
        using var last = new NativeHandler(0b11111);
        var controller = new CustomController();
        using var export = new ControllerExport(controller);
        export.Native->setComponentHandler(first.Handler);
        controller.OnSetHandler = () =>
        {
            controller.OnSetHandler = null;
            export.Native->setComponentHandler(last.Handler);
            if (throwAfterReentry) throw new InvalidOperationException("Failed after reentry.");
        };

        Assert.AreEqual(throwAfterReentry ? LibVst.ComResult.InternalError : LibVst.ComResult.Ok,
            export.Native->setComponentHandler(intermediate.Handler).Value);

        first.AssertBalanced();
        intermediate.AssertBalanced();
        Assert.AreEqual(7, last.ReferenceCount);
        controller.Handler!.BeginEdit(1);
        Assert.AreEqual(1, last.State->EditCalls);
        export.Native->setComponentHandler(null);
        last.AssertBalanced();
    }

    [TestCase(false)]
    [TestCase(true)]
    public void CleanupCannotReentrantlyAcquireAnotherHandler(bool finalRelease)
    {
        using var host = new NativeHandler(0b11111);
        using var rejected = new NativeHandler(0b11111);
        var controller = new CustomController();
        using var export = new ControllerExport(controller);
        export.Native->setComponentHandler(host.Handler);
        var reentrantResult = LibVst.ComResult.Ok;
        controller.OnSetHandler = () =>
        {
            controller.OnSetHandler = null;
            reentrantResult = export.Native->setComponentHandler(rejected.Handler).Value;
        };

        if (finalRelease) export.Dispose();
        else ((LibVst.IPluginBase*)export.Native)->terminate();

        Assert.AreEqual(LibVst.ComResult.InternalError, reentrantResult);
        Assert.IsNull(controller.Handler);
        host.AssertBalanced();
        rejected.AssertBalanced();
    }

    [TestCase(false)]
    [TestCase(true)]
    public void CleanupStillReleasesReferencesIfTheControllerRejectsDisconnection(bool finalRelease)
    {
        using var host = new NativeHandler(0b11111);
        var controller = new CustomController();
        using var export = new ControllerExport(controller);
        export.Native->setComponentHandler(host.Handler);
        controller.RejectHandler = true;

        if (finalRelease)
        {
            Assert.Throws<InvalidOperationException>(() => export.Dispose());
            Assert.IsNull(export.ComObject.Target);
        }
        else
        {
            Assert.AreEqual(LibVst.ComResult.InternalError, ((LibVst.IPluginBase*)export.Native)->terminate().Value);
        }

        host.AssertBalanced();
        Assert.Throws<ObjectDisposedException>(() => controller.Handler!.BeginEdit(1));
        // This deliberately misbehaving custom controller must be repaired before its own Dispose uses Handler.
        controller.RejectHandler = false;
        controller.SetControllerHandler(null);
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void ContextMenuCreationScopesTheViewReference(bool fail, bool retainView)
    {
        using var host = new NativeHandler(0b11111);
        var controller = new Controller();
        using var export = new ControllerExport(controller);
        export.Native->setComponentHandler(host.Handler);
        var view = new View();
        var viewComObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(view);
        var nativeView = viewComObject.QueryInterface<LibVst.IPlugView>();
        host.State->FailMenu = fail;
        host.State->RetainView = retainView;
        try
        {
            if (fail)
            {
                Assert.Throws<InvalidOperationException>(() => controller.Handler!.CreateContextMenu(view, 42));
                Assert.AreEqual(1, host.State->Menu.References);
            }
            else
            {
                using var menu = controller.Handler!.CreateContextMenu(view, 42);
                Assert.AreEqual(0, menu.GetItemCount());
                Assert.AreEqual(2, host.State->Menu.References, "The returned menu reference belongs to the managed wrapper.");
                menu.Dispose();
                menu.Dispose();
                Assert.AreEqual(1, host.State->Menu.References);
            }

            Assert.AreEqual((nint)nativeView, (nint)host.State->LastView);
            Assert.AreEqual(42u, host.State->LastParameterId);
            Assert.AreEqual(2u, host.State->ViewReferencesDuringCall, "The host must receive a live, temporarily retained view.");
            Assert.AreEqual(retainView ? 2u : 1u, viewComObject.ReferenceCount);
            host.ReleaseRetainedView();
            Assert.AreEqual(1u, viewComObject.ReferenceCount, "Only the caller's view reference should remain.");
        }
        finally
        {
            host.ReleaseRetainedView();
            // Also drain leaked references when this regression test runs against the unfixed implementation.
            while (viewComObject.ReferenceCount != 0) viewComObject.ReleaseRef();
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public void ContextMenusDisposeTargetlessItemsAndOwnedActionReferences(bool includeAction)
    {
        using var host = new NativeHandler(0b11111);
        var controller = new Controller();
        using var export = new ControllerExport(controller);
        export.Native->setComponentHandler(host.Handler);
        using var menu = controller.Handler!.CreateContextMenu(new View(), 42);
        LibVst.ComObject? actionObject = null;
        try
        {
            menu.AddItem(new AudioContextMenuItem("Heading"), null);
            menu.AddItem(new AudioContextMenuItem("", Flags: AudioContextMenuItemFlags.IsSeparator), null);
            if (includeAction)
            {
                AudioContextMenuAction action = _ => { };
                actionObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(action);
                menu.AddItem(new AudioContextMenuItem("Action", 7), action);
                Assert.AreEqual(1u, actionObject.ReferenceCount);
            }

            Assert.AreEqual(includeAction ? 3 : 2, menu.GetItemCount());
            Assert.DoesNotThrow(menu.Dispose);
            menu.Dispose();
            Assert.AreEqual(1, host.State->Menu.References, "Only the fake host's menu reference remains.");
            Assert.AreEqual(1, host.State->MenuReleaseCalls, "The wrapper must release the native menu exactly once.");
            if (actionObject is not null) Assert.AreEqual(0u, actionObject.ReferenceCount);
        }
        finally
        {
            menu.Dispose();
            // Also drain action references when running this regression against the unfixed implementation.
            if (actionObject is not null)
            {
                while (actionObject.ReferenceCount != 0) actionObject.ReleaseRef();
            }
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public void DetachedHandlersDoNotQueryTransportInterfaces(bool terminate)
    {
        using var host = new NativeHandler(0b11111);
        var controller = new Controller();
        using var export = new ControllerExport(controller);
        export.Native->setComponentHandler(host.Handler);
        var previous = controller.Handler!;
        if (terminate) ((LibVst.IPluginBase*)export.Native)->terminate();
        else export.Native->setComponentHandler(null);

        var queryCount = host.State->QueryCalls;
        Assert.IsFalse(previous.IsTransportControlSupported);
        Assert.IsFalse(previous.IsTransportActionSupported(AudioTransportAction.PlaybackStart));
        Assert.AreEqual(queryCount, host.State->QueryCalls, "A disposed handler must not query native interfaces.");
        Assert.Throws<ObjectDisposedException>(() => previous.RequestTransportAction(AudioTransportAction.PlaybackStart));
        Assert.Throws<ObjectDisposedException>(() => previous.RequestTransportAction(AudioTransportAction.Locate, AudioTransportPosition.FromSamples(0)));
        host.AssertBalanced();
    }

    private sealed class ControllerExport : IDisposable
    {
        private bool _disposed;

        public ControllerExport(IAudioController controller)
        {
            ComObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(controller);
            Native = ComObject.QueryInterface<LibVst.IEditController>();
        }

        public LibVst.ComObject ComObject { get; }
        public LibVst.IEditController* Native { get; }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            ComObject.ReleaseRef();
        }
    }

    /// <summary>
    /// A host with distinct interface pointers sharing one reference count. Each successful query retains exactly
    /// the returned interface. The host keeps its initial reference until explicitly released or disposed.
    /// </summary>
    private sealed class NativeHandler : IDisposable
    {
        private bool _ownsHostReference = true;
        public readonly NativeState* State;

        public NativeHandler(int supportedInterfaces)
        {
            State = (NativeState*)NativeMemory.AllocZeroed((nuint)sizeof(NativeState));
            State->SupportedInterfaces = supportedInterfaces;
            State->ReferenceCount = 1;
            State->OwnerThreadId = Environment.CurrentManagedThreadId;
            var interfaces = &State->Handler;
            for (int i = 0; i < 7; i++)
            {
                var nativeInterface = interfaces + i;
                nativeInterface->Owner = State;
                nativeInterface->Vtbl = (void**)NativeMemory.AllocZeroed((nuint)(8 * sizeof(void*)));
                nativeInterface->Vtbl[0] = (delegate* unmanaged[MemberFunction]<NativeInterface*, Guid*, void**, int>)&QueryInterface;
                nativeInterface->Vtbl[1] = (delegate* unmanaged[MemberFunction]<NativeInterface*, uint>)&AddRef;
                nativeInterface->Vtbl[2] = (delegate* unmanaged[MemberFunction]<NativeInterface*, uint>)&Release;
            }
            State->Handler.References = 1;
            State->Menu.References = 1; // The fake host's own reference, separate from a returned menu reference.
            State->Handler.Vtbl[3] = (delegate* unmanaged[MemberFunction]<NativeInterface*, uint, int>)&BeginEdit;
            State->Handler3.Vtbl[3] = (delegate* unmanaged[MemberFunction]<NativeInterface*, LibVst.IPlugView*, uint*, LibVst.IContextMenu*>)&CreateMenu;
            State->Menu.Vtbl[3] = (delegate* unmanaged[MemberFunction]<NativeInterface*, int>)&GetItemCount;
            State->Menu.Vtbl[5] = (delegate* unmanaged[MemberFunction]<NativeInterface*, LibVst.Item*, LibVst.IContextMenuTarget*, int>)&AddMenuItem;
        }

        public LibVst.IComponentHandler* Handler => (LibVst.IComponentHandler*)&State->Handler;
        public int ReferenceCount => State->ReferenceCount;

        public void ReleaseHostReference()
        {
            if (!_ownsHostReference) return;
            _ownsHostReference = false;
            Handler->release();
        }

        public void ReleaseRetainedView()
        {
            if (State->RetainedView == null) return;
            State->RetainedView->release();
            State->RetainedView = null;
        }

        public void AssertBalanced()
        {
            Assert.AreEqual(_ownsHostReference ? 1 : 0, ReferenceCount);
            Assert.AreEqual(_ownsHostReference ? 1 : 0, State->Handler.References);
            var optionalInterfaces = &State->Handler2;
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(0, optionalInterfaces[i].References, $"Optional interface {i} leaked or was over-released.");
            }
            Assert.IsFalse(State->UsedAfterFinalRelease);
            Assert.AreEqual(0, State->WrongThreadReleases, "Releases must occur on the calling thread, not the finalizer thread.");
        }

        public void Dispose()
        {
            ReleaseRetainedView();
            ReleaseHostReference();
            var interfaces = &State->Handler;
            for (int i = 0; i < 7; i++) NativeMemory.Free(interfaces[i].Vtbl);
            NativeMemory.Free(State);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        private static int QueryInterface(NativeInterface* self, Guid* iid, void** obj)
        {
            var state = self->Owner;
            state->QueryCalls++;
            *obj = null;
            NativeInterface* result = null;
            if (*iid == LibVst.FUnknown.IId || *iid == LibVst.IComponentHandler.IId) result = &state->Handler;
            else if (*iid == LibVst.IComponentHandler2.IId && (state->SupportedInterfaces & 1) != 0) result = &state->Handler2;
            else if (*iid == LibVst.IComponentHandler3.IId && (state->SupportedInterfaces & 2) != 0) result = &state->Handler3;
            else if (*iid == LibVst.IComponentHandlerBusActivation.IId && (state->SupportedInterfaces & 4) != 0) result = &state->BusActivation;
            else if (*iid == LibVst.IProgress.IId && (state->SupportedInterfaces & 8) != 0) result = &state->Progress;
            else if (*iid == LibVst.IUnitHandler.IId && (state->SupportedInterfaces & 16) != 0) result = &state->UnitHandler;
            if (result == null) return LibVst.ComResult.NoInterface;
            Retain(result);
            *obj = result;
            return LibVst.ComResult.Ok;
        }

        private static uint Retain(NativeInterface* self)
        {
            var state = self->Owner;
            self->References++;
            if (self == &state->Menu) return (uint)self->References;
            if (state->ReferenceCount == 0) state->UsedAfterFinalRelease = true;
            return (uint)++state->ReferenceCount;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        private static uint AddRef(NativeInterface* self) => Retain(self);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        private static uint Release(NativeInterface* self)
        {
            var state = self->Owner;
            self->References--;
            state->ReleaseCalls++;
            if (Environment.CurrentManagedThreadId != state->OwnerThreadId) state->WrongThreadReleases++;
            if (self == &state->Menu)
            {
                state->MenuReleaseCalls++;
                return (uint)self->References;
            }
            return (uint)--state->ReferenceCount;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        private static int BeginEdit(NativeInterface* self, uint id)
        {
            if (self->Owner->ReferenceCount == 0) self->Owner->UsedAfterFinalRelease = true;
            self->Owner->EditCalls++;
            return LibVst.ComResult.Ok;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        private static LibVst.IContextMenu* CreateMenu(NativeInterface* self, LibVst.IPlugView* view, uint* paramId)
        {
            var state = self->Owner;
            state->LastView = view;
            state->LastParameterId = *paramId;
            state->ViewReferencesDuringCall = ((LibVst.ComObjectHandle*)view)->ComObject.ReferenceCount;
            if (state->RetainView)
            {
                view->addRef();
                state->RetainedView = view;
            }
            if (state->FailMenu) return null;
            Retain(&state->Menu);
            return (LibVst.IContextMenu*)&state->Menu;
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        private static int GetItemCount(NativeInterface* self) => self->Owner->MenuItemCount;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvMemberFunction)])]
        private static int AddMenuItem(NativeInterface* self, LibVst.Item* item, LibVst.IContextMenuTarget* target)
        {
            self->Owner->MenuItemCount++;
            return LibVst.ComResult.Ok;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeInterface
    {
        public void** Vtbl;
        public NativeState* Owner;
        public int References;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeState
    {
        public NativeInterface Handler;
        public NativeInterface Handler2;
        public NativeInterface Handler3;
        public NativeInterface BusActivation;
        public NativeInterface Progress;
        public NativeInterface UnitHandler;
        public NativeInterface Menu;
        public int SupportedInterfaces;
        public int ReferenceCount;
        public int OwnerThreadId;
        public int WrongThreadReleases;
        public int ReleaseCalls;
        public int QueryCalls;
        public int MenuItemCount;
        public int MenuReleaseCalls;
        public int EditCalls;
        public bool UsedAfterFinalRelease;
        public bool FailMenu;
        public bool RetainView;
        public LibVst.IPlugView* LastView;
        public LibVst.IPlugView* RetainedView;
        public uint LastParameterId;
        public uint ViewReferencesDuringCall;
    }

    private sealed class Model : AudioProcessorModel
    {
    }

    private sealed class Controller : AudioController<Model>
    {
        public bool ThrowOnTerminate { get; init; }
        public bool HadHandlerDuringTermination { get; private set; }

        protected override void Terminate()
        {
            HadHandlerDuringTermination = Handler is not null;
            Handler?.BeginEdit(1);
            if (ThrowOnTerminate) throw new InvalidOperationException("Termination failed.");
        }
    }

    // A direct implementation verifies that ownership does not depend on inheriting AudioController<T>.
    private sealed class CustomController : IAudioController, IDisposable
    {
        public IAudioControllerHandler? Handler { get; private set; }
        public bool RejectHandler { get; set; }
        public Action? OnSetHandler { get; set; }
        public bool ThrowOnDispose { get; init; }
        public bool HadHandlerDuringDisposal { get; private set; }
        public AudioHostApplication? Host => null;
        public int ParameterCount => 0;
        public bool Initialize(AudioHostApplication hostApplication) => true;
        public void Terminate() { }
        public void SetComponentState(Stream streamInput) { }
        public void SetState(Stream streamInput) { }
        public void GetState(Stream streamOutput) { }
        public AudioParameterInfo GetParameterInfo(int paramIndex) => default;
        public string GetParameterStringByValue(AudioParameterId id, double valueNormalized) => "";
        public double GetParameterValueByString(AudioParameterId id, string valueAsString) => 0;
        public double NormalizedParameterToPlain(AudioParameterId id, double valueNormalized) => valueNormalized;
        public double PlainParameterToNormalized(AudioParameterId id, double plainValue) => plainValue;
        public double GetParameterNormalized(AudioParameterId id) => 0;
        public void SetParameterNormalized(AudioParameterId id, double valueNormalized) { }
        public IAudioPluginView? CreateView() => null;

        public void SetControllerHandler(IAudioControllerHandler? controllerHandler)
        {
            if (RejectHandler) throw new InvalidOperationException("Handler rejected.");
            Handler = controllerHandler;
            OnSetHandler?.Invoke();
        }

        public void Dispose()
        {
            HadHandlerDuringDisposal = Handler is not null;
            Handler?.BeginEdit(1);
            if (ThrowOnDispose) throw new InvalidOperationException("Disposal failed.");
        }
    }

    private sealed class View : IAudioPluginView
    {
        public bool IsPlatformTypeSupported(AudioPluginViewPlatform platform) => true;
        public void Attached(nint parent, AudioPluginViewPlatform type) { }
        public void Removed() { }
        public bool OnWheel(float distance) => false;
        public bool OnKeyDown(ushort key, short keyCode, short modifiers) => false;
        public bool OnKeyUp(ushort key, short keyCode, short modifiers) => false;
        public ViewRectangle Size => new(0, 0, 100, 100);
        public void OnSize(ViewRectangle newSize) { }
        public void OnFocus(bool state) { }
        public void SetFrame(IAudioPluginFrame? frame) { }
        public bool CanResize() => false;
        public bool CheckSizeConstraint(ref ViewRectangle rect) => true;
        public bool SetContentScaleFactor(float factor) => false;
        public bool TryFindParameter(int xPos, int yPos, out AudioParameterId parameterId)
        {
            parameterId = default;
            return false;
        }
    }
}
