// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NPlug.Interop;
using NPlug.Validator;
using NUnit.Framework;

namespace NPlug.Tests;

/// <summary>
/// Strings and messages exchanged with the host: <see cref="AudioMessage"/>s sent between the controller and the
/// processor through the host of the official VST3 validator (host-created IMessage, IConnectionPoint::notify), and
/// UTF-16 strings received from the host (IEditController::getParamValueByString).
/// </summary>
public unsafe class TestAudioMessages
{
    private static readonly Lock ReceivedLock = new();
    private static readonly List<string> Received = new();
    private static readonly List<string> Strings = new();

    [Test]
    public void MessagesCarryTheirIdAndAttributes()
    {
        lock (ReceivedLock) Received.Clear();

        var factory = new AudioPluginFactory(new AudioPluginFactoryInfo("NPlug", "https://github.com/xoofx/NPlug", "noreply@localhost"));
        factory.RegisterPlugin<MessageProcessor>(new AudioProcessorClassInfo(MessageProcessor.ClassId, "MessageTest", AudioProcessorCategory.Effect));
        factory.RegisterPlugin<MessageController>(new AudioControllerClassInfo(MessageController.ClassId, "MessageTest Controller"));

        var output = new StringWriter();
        var errors = new StringWriter();
        var result = AudioPluginValidator.Validate(factory.Export, output, errors);
        if (!result) TestContext.Out.WriteLine(output.ToString());
        Assert.True(result, "The validator failed, check the logs.");

        List<string> received;
        lock (ReceivedLock) received = new List<string>(Received);
        Assert.IsNotEmpty(received, "The processor received no message from the controller.");
        foreach (var message in received)
        {
            Assert.AreEqual("NPlug.Test.Ping|42|0.25|héllo|1,2,3", message);
        }
    }

    [Test]
    public void ParameterValueByStringReceivesTheHostText()
    {
        lock (ReceivedLock) Strings.Clear();
        var controller = new MessageController();
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(controller);
        var host = FakeHost.Create();
        try
        {
            var pluginBase = comObject.QueryInterface<LibVst.IPluginBase>();
            Assert.AreEqual(LibVst.ComResult.Ok, pluginBase->initialize((LibVst.FUnknown*)host).Value);
            var editController = comObject.QueryInterface<LibVst.IEditController>();

            var text = stackalloc char[128];
            "0.75".AsSpan().CopyTo(new Span<char>(text, 128));
            text[4] = '\0';
            var probe = controller.Model.GetParameterByIndex(1);
            LibVst.ParamValue value;
            Assert.AreEqual(LibVst.ComResult.Ok, editController->getParamValueByString(new LibVst.ParamID((uint)probe.Id.Value), text, &value).Value);

            lock (ReceivedLock) Assert.AreEqual(new[] { "0.75" }, Strings.ToArray(), "The host text must reach the parameter unchanged.");
            pluginBase->terminate();
        }
        finally
        {
            comObject.ReleaseRef();
            FakeHost.Free(host);
        }
    }

    /// <summary>Minimal native IHostApplication: answers every interface query with itself, creates nothing.</summary>
    private static class FakeHost
    {
        public static void** Create()
        {
            var vtbl = (void**)NativeMemory.Alloc((nuint)(5 * sizeof(void*)));
            vtbl[0] = (delegate* unmanaged[MemberFunction]<void*, Guid*, void**, int>)&QueryInterface;
            vtbl[1] = (delegate* unmanaged[MemberFunction]<void*, uint>)&AddRef;
            vtbl[2] = (delegate* unmanaged[MemberFunction]<void*, uint>)&Release;
            vtbl[3] = (delegate* unmanaged[MemberFunction]<void*, char*, int>)&GetName;
            vtbl[4] = (delegate* unmanaged[MemberFunction]<void*, Guid*, Guid*, void**, int>)&CreateInstance;
            var self = (void**)NativeMemory.Alloc((nuint)sizeof(void*));
            *self = vtbl;
            return self;
        }

        public static void Free(void** self)
        {
            NativeMemory.Free(*self);
            NativeMemory.Free(self);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvMemberFunction) })]
        private static int QueryInterface(void* self, Guid* iid, void** obj)
        {
            *obj = self;
            return 0;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvMemberFunction) })]
        private static uint AddRef(void* self) => 1;

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvMemberFunction) })]
        private static uint Release(void* self) => 1;

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvMemberFunction) })]
        private static int GetName(void* self, char* name)
        {
            "FakeHost\0".AsSpan().CopyTo(new Span<char>(name, 128));
            return 0;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvMemberFunction) })]
        private static int CreateInstance(void* self, Guid* cid, Guid* iid, void** obj)
        {
            *obj = null;
            return 1; // kResultFalse
        }
    }

    private sealed class MessageModel : AudioProcessorModel
    {
        public MessageModel() : base("NPlug.Tests.Messages")
        {
            AddByPassParameter();
            AddParameter(new ProbeParameter());
        }
    }

    private sealed class ProbeParameter : AudioParameter
    {
        public ProbeParameter() : base("Probe")
        {
        }

        public override double FromString(string plainValueAsString)
        {
            lock (ReceivedLock) Strings.Add(plainValueAsString);
            return base.FromString(plainValueAsString);
        }
    }

    private sealed class MessageProcessor : AudioProcessor<MessageModel>
    {
        public static readonly Guid ClassId = new("6a0d8b8e-7f0e-4d6f-9a43-2f6f0c2b9d11");

        public MessageProcessor() : base(AudioSampleSizeSupport.Float32)
        {
        }

        public override Guid ControllerClassId => MessageController.ClassId;

        protected override bool Initialize(AudioHostApplication host)
        {
            AddDefaultStereoAudioInput();
            AddDefaultStereoAudioOutput();
            return true;
        }

        protected override void OnMessage(AudioMessage message)
        {
            var attributes = message.Attributes;
            attributes.TryGetInt64("int", out var intValue);
            attributes.TryGetFloat64("float", out var floatValue);
            attributes.TryGetString("text", out var text);
            attributes.TryGetBinary("bytes", out var bytes);
            var entry = $"{message.Id}|{intValue}|{floatValue.ToString(System.Globalization.CultureInfo.InvariantCulture)}|{text}|{string.Join(",", bytes.ToArray())}";
            lock (ReceivedLock) Received.Add(entry);
        }
    }

    private sealed class MessageController : AudioController<MessageModel>
    {
        public static readonly Guid ClassId = new("0c3f6e2a-5b1d-4f8e-8a7c-9d2e4b6f1a35");

        protected override void OnConnect(IAudioConnectionPoint connectionPoint)
        {
            Assert.IsNotNull(Host);
            Assert.True(Host!.TryCreateMessage("NPlug.Test.Ping", out var message), "The host could not create a message.");
            try
            {
                var attributes = message.Attributes;
                Assert.True(attributes.TrySetInt64("int", 42));
                Assert.True(attributes.TrySetFloat64("float", 0.25));
                Assert.True(attributes.TrySetString("text", "héllo"));
                Assert.True(attributes.TrySetBinary("bytes", new byte[] { 1, 2, 3 }));
                Assert.AreEqual("NPlug.Test.Ping", message.Id);
                connectionPoint.Notify(message);
            }
            finally
            {
                message.Dispose();
            }
        }
    }
}
