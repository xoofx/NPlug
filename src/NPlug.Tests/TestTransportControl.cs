// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NPlug.Interop;
using NUnit.Framework;

namespace NPlug.Tests;

/// <summary>
/// Transport requests from the plugin to the host (<c>Vst::ITransportControl</c>, VST 3.8.1). The host objects below
/// are implemented with the C++ signatures: the component handler answers <c>queryInterface</c> for
/// <c>ITransportControl</c> when the host supports it.
/// </summary>
public unsafe class TestTransportControl
{
    private static readonly List<string> Calls = new();
    private static void* _transportControl;
    private static uint _transportReferenceCount;

    // IComponentHandler::queryInterface: only ITransportControl is known, when the host supports it.
    [UnmanagedCallersOnly]
    private static int HandlerQueryInterface(void* self, Guid* iid, void** obj)
    {
        if (_transportControl != null && *iid == LibVst.ITransportControl.IId)
        {
            *obj = _transportControl;
            ((LibVst.ITransportControl*)_transportControl)->addRef();
            return 0; // kResultOk
        }
        *obj = null;
        return unchecked((int)0x80004002); // kNoInterface
    }

    [UnmanagedCallersOnly]
    private static int QueryInterface(void* self, Guid* iid, void** obj)
    {
        *obj = null;
        return unchecked((int)0x80004002); // kNoInterface
    }

    [UnmanagedCallersOnly]
    private static uint AddRefOrRelease(void* self) => 1;

    [UnmanagedCallersOnly]
    private static uint TransportAddRef(void* self) => ++_transportReferenceCount;

    [UnmanagedCallersOnly]
    private static uint TransportRelease(void* self) => --_transportReferenceCount;

    // C++ layout of Vst::TransportPosition: int32 type, then a union of TSamples (int64) and TQuarterNotes (double) at offset 8.
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    private struct CppTransportPosition
    {
        [FieldOffset(0)] public int Type;
        [FieldOffset(8)] public long Samples;
        [FieldOffset(8)] public double QuarterNotes;
    }

    // ITransportControl::requestAction (int32 action, TransportPosition* position)
    [UnmanagedCallersOnly]
    private static int RequestAction(void* self, int action, CppTransportPosition* position)
    {
        if (position is null)
        {
            Calls.Add($"requestAction {action}");
        }
        else
        {
            Calls.Add(position->Type == 0 ? $"requestAction {action} samples {position->Samples}" : $"requestAction {action} quarterNotes {position->QuarterNotes}");
        }
        // The host refuses to record.
        return action == (int)AudioTransportAction.RecordOnPlaybackStart ? 1 /* kResultFalse */ : 0;
    }

    // ITransportControl::isActionSupported (int32 action): this host has no cycle.
    [UnmanagedCallersOnly]
    private static int IsActionSupported(void* self, int action) => action >= (int)AudioTransportAction.SetCycleStart ? 1 : 0;

    private static void** CreateNativeObject(int slotCount, delegate* unmanaged<void*, Guid*, void**, int> queryInterface)
    {
        var vtable = (void**)NativeMemory.AllocZeroed((nuint)(slotCount * sizeof(void*)));
        vtable[0] = queryInterface;
        vtable[1] = (delegate* unmanaged<void*, uint>)&AddRefOrRelease;
        vtable[2] = (delegate* unmanaged<void*, uint>)&AddRefOrRelease;
        var obj = (void**)NativeMemory.AllocZeroed((nuint)sizeof(void*));
        obj[0] = vtable;
        return obj;
    }

    private static void Free(void* obj)
    {
        if (obj is null) return;
        NativeMemory.Free(*(void**)obj);
        NativeMemory.Free(obj);
    }

    [SetUp]
    public void SetUp() => Calls.Clear();

    [TearDown]
    public void TearDown()
    {
        Free(_transportControl);
        _transportControl = null;
    }

    private static (EmptyController Controller, LibVst.ComObject ComObject, nint Handler) Connect(bool transportControl)
    {
        if (transportControl)
        {
            var transport = CreateNativeObject(5, &QueryInterface);
            var vtable = *(void***)transport;
            vtable[1] = (delegate* unmanaged<void*, uint>)&TransportAddRef;
            vtable[2] = (delegate* unmanaged<void*, uint>)&TransportRelease;
            vtable[3] = (delegate* unmanaged<void*, int, CppTransportPosition*, int>)&RequestAction;
            vtable[4] = (delegate* unmanaged<void*, int, int>)&IsActionSupported;
            _transportReferenceCount = 1; // The host owns the initial reference.
            _transportControl = transport;
        }
        var handler = CreateNativeObject(8, &HandlerQueryInterface);
        var controller = new EmptyController();
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(controller);
        var editController = comObject.QueryInterface<LibVst.IEditController>();
        Assert.AreEqual(LibVst.ComResult.Ok, editController->setComponentHandler((LibVst.IComponentHandler*)handler).Value);
        return (controller, comObject, (nint)handler);
    }

    private static void Disconnect(LibVst.ComObject comObject, nint handler)
    {
        var editController = comObject.QueryInterface<LibVst.IEditController>();
        editController->setComponentHandler(null);
        editController->release();
        comObject.ReleaseRef();
        Free((void*)handler);
    }

    [Test]
    public void TransportPositionHasTheCppLayout()
    {
        Assert.AreEqual(16, Unsafe.SizeOf<LibVst.TransportPosition>());
        var position = new LibVst.TransportPosition();
        Assert.AreEqual(8, (int)((byte*)&position.union - (byte*)&position));
    }

    [Test]
    public void RequestsReachTheHostWithTheirPosition()
    {
        var (controller, comObject, handler) = Connect(transportControl: true);
        try
        {
            var host = controller.Handler!;
            Assert.IsTrue(host.IsTransportControlSupported);
            Assert.IsTrue(host.RequestTransportAction(AudioTransportAction.PlaybackStart));
            Assert.IsTrue(host.RequestTransportAction(AudioTransportAction.Locate, AudioTransportPosition.FromSamples(48000)));
            Assert.IsTrue(host.RequestTransportAction(AudioTransportAction.LocateAndPlaybackStart, AudioTransportPosition.FromQuarterNotes(16.5)));
            Assert.IsFalse(host.RequestTransportAction(AudioTransportAction.RecordOnPlaybackStart), "the host refuses");
            CollectionAssert.AreEqual(new[]
            {
                "requestAction 1",
                "requestAction 0 samples 48000",
                "requestAction 3 quarterNotes 16.5",
                "requestAction 5",
            }, Calls);

            Assert.IsTrue(host.IsTransportActionSupported(AudioTransportAction.PlaybackStop));
            Assert.IsFalse(host.IsTransportActionSupported(AudioTransportAction.CycleOn));
            Assert.Throws<ArgumentException>(() => host.RequestTransportAction(AudioTransportAction.SetCycleStart), "a position is required");
        }
        finally
        {
            Disconnect(comObject, handler);
        }
    }

    [Test]
    public void HostsWithoutTransportControlAreDetected()
    {
        var (controller, comObject, handler) = Connect(transportControl: false);
        try
        {
            var host = controller.Handler!;
            Assert.IsFalse(host.IsTransportControlSupported);
            Assert.IsFalse(host.IsTransportActionSupported(AudioTransportAction.PlaybackStart));
            Assert.Throws<NotSupportedException>(() => host.RequestTransportAction(AudioTransportAction.PlaybackStart));
            Assert.Throws<NotSupportedException>(() => host.RequestTransportAction(AudioTransportAction.Locate, AudioTransportPosition.FromSamples(0)));
        }
        finally
        {
            Disconnect(comObject, handler);
        }
    }

    [Test]
    public void TransportQueriesDoNotRetainHostReferences()
    {
        var (controller, comObject, handler) = Connect(transportControl: true);
        try
        {
            Assert.That(_transportReferenceCount, Is.EqualTo(1), "connecting must not retain a transport reference");
            var host = controller.Handler!;
            Assert.IsTrue(host.IsTransportControlSupported);
            Assert.That(_transportReferenceCount, Is.EqualTo(1), "capability queries must release their reference");
            Assert.IsTrue(host.IsTransportActionSupported(AudioTransportAction.PlaybackStart));
            Assert.IsFalse(host.IsTransportActionSupported(AudioTransportAction.CycleOn));
            Assert.That(_transportReferenceCount, Is.EqualTo(1), "action queries must release their reference");
            Assert.IsTrue(host.RequestTransportAction(AudioTransportAction.PlaybackStart));
            Assert.IsFalse(host.RequestTransportAction(AudioTransportAction.RecordOnPlaybackStart));
            Assert.That(_transportReferenceCount, Is.EqualTo(1), "accepted and refused requests must release their reference");
            Assert.IsTrue(host.RequestTransportAction(AudioTransportAction.Locate, AudioTransportPosition.FromSamples(48000)));
            Assert.IsFalse(host.RequestTransportAction(AudioTransportAction.RecordOnPlaybackStart, AudioTransportPosition.FromSamples(0)));
            Assert.That(_transportReferenceCount, Is.EqualTo(1), "position requests must release their reference");

            var editController = comObject.QueryInterface<LibVst.IEditController>();
            try
            {
                editController->setComponentHandler((LibVst.IComponentHandler*)handler);
            }
            finally
            {
                editController->release();
            }
            Assert.That(_transportReferenceCount, Is.EqualTo(1), "replacing a handler must not retain a transport reference");
        }
        finally
        {
            Disconnect(comObject, handler);
        }
        Assert.That(_transportReferenceCount, Is.EqualTo(1), "disconnecting must leave only the host's reference");
        Assert.That(comObject.ReferenceCount, Is.Zero, "the test controller must also release all its references");
    }

    [Test]
    public void ActionsRequiringAPositionAreKnown()
    {
        var withPosition = Enum.GetValues<AudioTransportAction>().Where(a => a.RequiresPosition()).ToArray();
        CollectionAssert.AreEqual(new[]
        {
            AudioTransportAction.Locate, AudioTransportAction.LocateAndPlaybackStart, AudioTransportAction.PlaybackStopAndLocate,
            AudioTransportAction.LocateAndRecordOnPlaybackStart, AudioTransportAction.RecordOffPlaybackStopAndLocate,
            AudioTransportAction.SetCycleStart, AudioTransportAction.SetCycleEnd,
        }, withPosition);
        Assert.AreEqual((int)LibVst.ITransportControl.Action.CycleOff, (int)AudioTransportAction.CycleOff, "same values as the SDK");
    }

    private sealed class EmptyModel : AudioProcessorModel
    {
        public EmptyModel() : base("Empty")
        {
        }
    }

    private sealed class EmptyController : AudioController<EmptyModel>
    {
    }
}
