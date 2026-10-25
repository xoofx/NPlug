// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NPlug.Interop;
using NUnit.Framework;

namespace NPlug.Tests;

/// <summary>
/// Calls made by the plugin to host objects must follow the C++ signatures: a VST3 typedef of <c>double</c> or
/// <c>float</c> (<c>ParamValue</c>, <c>ScaleFactor</c>...) is a plain floating-point argument for the host. The host
/// objects below are implemented with those plain C++ signatures.
/// </summary>
/// <remarks>
/// On Windows x64, a struct wrapping a <c>double</c> passed by value travels in an integer register while a
/// <c>double</c> travels in an XMM register, so passing the generated wrapper struct makes the host read another
/// register (it typically receives 0). The System V ABI (Linux/macOS x64) and ARM64 pass both the same way, so these
/// tests only fail on Windows x64 when the generated function pointers use the wrapper structs.
/// </remarks>
public unsafe class TestNativeCallAbi
{
    private static readonly List<string> Calls = new();

    [UnmanagedCallersOnly]
    private static int QueryInterface(void* self, Guid* iid, void** obj)
    {
        *obj = null;
        return unchecked((int)0x80004002); // kNoInterface
    }

    [UnmanagedCallersOnly]
    private static uint AddRefOrRelease(void* self) => 1;

    // IComponentHandler::performEdit (ParamID id, ParamValue valueNormalized)
    [UnmanagedCallersOnly]
    private static int PerformEdit(void* self, uint id, double valueNormalized)
    {
        Calls.Add($"performEdit {id} {valueNormalized}");
        return 0;
    }

    // IParamValueQueue::addPoint (int32 sampleOffset, ParamValue value, int32& index)
    [UnmanagedCallersOnly]
    private static int AddPoint(void* self, int sampleOffset, double value, int* index)
    {
        *index = 7;
        Calls.Add($"addPoint {sampleOffset} {value}");
        return 0;
    }

    // IProgress::update (ID id, ParamValue normValue)
    [UnmanagedCallersOnly]
    private static int Update(void* self, ulong id, double normValue)
    {
        Calls.Add($"update {id} {normValue}");
        return 0;
    }

    // IPlugViewContentScaleSupport::setContentScaleFactor (ScaleFactor factor)
    [UnmanagedCallersOnly]
    private static int SetContentScaleFactor(void* self, float factor)
    {
        Calls.Add($"setContentScaleFactor {factor}");
        return 0;
    }

    /// <summary>Allocates a native object whose vtable holds FUnknown and <paramref name="method"/> at <paramref name="slot"/>.</summary>
    private static T* CreateNativeObject<T>(int slot, void* method) where T : unmanaged
    {
        var vtable = (void**)NativeMemory.AllocZeroed((nuint)((slot + 1) * sizeof(void*)));
        vtable[0] = (delegate* unmanaged<void*, Guid*, void**, int>)&QueryInterface;
        vtable[1] = (delegate* unmanaged<void*, uint>)&AddRefOrRelease;
        vtable[2] = (delegate* unmanaged<void*, uint>)&AddRefOrRelease;
        vtable[slot] = method;
        var obj = (void**)NativeMemory.AllocZeroed((nuint)sizeof(void*));
        obj[0] = vtable;
        return (T*)obj;
    }

    private static void Free(void* obj)
    {
        NativeMemory.Free(*(void**)obj);
        NativeMemory.Free(obj);
    }

    [SetUp]
    public void ClearCalls() => Calls.Clear();

    [Test]
    public void PerformEditPassesTheValueAsADouble()
    {
        var handler = CreateNativeObject<LibVst.IComponentHandler>(4, (delegate* unmanaged<void*, uint, double, int>)&PerformEdit);
        try
        {
            handler->performEdit(new LibVst.ParamID(5), new LibVst.ParamValue(0.25));
            CollectionAssert.AreEqual(new[] { "performEdit 5 0.25" }, Calls);
        }
        finally
        {
            Free(handler);
        }
    }

    [Test]
    public void ControllerEditsReachTheNativeHandlerWithTheirValue()
    {
        var native = CreateNativeObject<LibVst.IComponentHandler>(6, null);
        var vtable = *(void***)native;
        vtable[3] = (delegate* unmanaged<void*, uint, int>)&BeginOrEndEdit;
        vtable[4] = (delegate* unmanaged<void*, uint, double, int>)&PerformEdit;
        vtable[5] = (delegate* unmanaged<void*, uint, int>)&BeginOrEndEdit;
        vtable[6] = (delegate* unmanaged<void*, int, int>)&RestartComponent;
        var controller = new OneParameterController();
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(controller);
        var editController = comObject.QueryInterface<LibVst.IEditController>();
        try
        {
            // What a host does: hand its IComponentHandler to the controller, then the user edits a parameter.
            Assert.AreEqual(LibVst.ComResult.Ok, editController->setComponentHandler((LibVst.IComponentHandler*)native).Value);
            var a = controller.Model.A;
            controller.BeginEditParameter(a);
            a.NormalizedValue = 0.75;
            controller.EndEditParameter(a);
            CollectionAssert.AreEqual(new[] { $"edit {a.Id.Value}", $"performEdit {a.Id.Value} 0.75", $"edit {a.Id.Value}" }, Calls);
            editController->setComponentHandler(null);
        }
        finally
        {
            comObject.ReleaseRef();
            Free(native);
        }
    }

    [UnmanagedCallersOnly]
    private static int BeginOrEndEdit(void* self, uint id)
    {
        Calls.Add($"edit {id}");
        return 0;
    }

    [UnmanagedCallersOnly]
    private static int RestartComponent(void* self, int flags) => 0;

    [Test]
    public void AddPointPassesTheValueAsADouble()
    {
        var queue = CreateNativeObject<LibVst.IParamValueQueue>(6, (delegate* unmanaged<void*, int, double, int*, int>)&AddPoint);
        try
        {
            int index;
            queue->addPoint(12, new LibVst.ParamValue(0.5), &index);
            Assert.AreEqual(7, index);
            CollectionAssert.AreEqual(new[] { "addPoint 12 0.5" }, Calls);
        }
        finally
        {
            Free(queue);
        }
    }

    [Test]
    public void ProgressUpdatePassesTheValueAsADouble()
    {
        var progress = CreateNativeObject<LibVst.IProgress>(4, (delegate* unmanaged<void*, ulong, double, int>)&Update);
        try
        {
            progress->update(new LibVst.ID(3), new LibVst.ParamValue(0.125));
            CollectionAssert.AreEqual(new[] { "update 3 0.125" }, Calls);
        }
        finally
        {
            Free(progress);
        }
    }

    [Test]
    public void SetContentScaleFactorPassesTheFactorAsAFloat()
    {
        var scaleSupport = CreateNativeObject<LibVst.IPlugViewContentScaleSupport>(3, (delegate* unmanaged<void*, float, int>)&SetContentScaleFactor);
        try
        {
            scaleSupport->setContentScaleFactor(new LibVst.ScaleFactor(1.5f));
            CollectionAssert.AreEqual(new[] { "setContentScaleFactor 1.5" }, Calls);
        }
        finally
        {
            Free(scaleSupport);
        }
    }

    private sealed class OneParameterModel : AudioProcessorModel
    {
        public OneParameterModel() : base("One")
        {
            A = AddParameter(new AudioParameter("A"));
        }

        public AudioParameter A { get; }
    }

    private sealed class OneParameterController : AudioController<OneParameterModel>
    {
    }
}
