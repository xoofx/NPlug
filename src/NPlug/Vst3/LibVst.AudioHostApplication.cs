// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    private sealed class AudioHostApplicationVst : AudioHostApplication
    {
        private readonly IHostApplication* _hostApplication;
        private readonly Dictionary<ulong, string> _nativeUTF8ToManaged;
        private readonly Dictionary<string, IntPtr> _managedToNativeUTF8;
        private readonly IntPtr _tempBuffer;

        public AudioHostApplicationVst(IHostApplication* hostApplication, string name) : base(name)
        {
            _hostApplication = hostApplication;
            _nativeUTF8ToManaged = new Dictionary<ulong, string>();
            _managedToNativeUTF8 = new Dictionary<string, IntPtr>();
            _tempBuffer = (IntPtr)NativeMemory.Alloc((nuint)TempBufferSize);
        }

        public IntPtr TempBuffer => _tempBuffer;

        public const int TempBufferSize = 4096;

        public override AudioMessage CreateMessage(string messageId)
        {
            IMessage* obj;
            var result = _hostApplication->createInstance(IMessage.IId, IMessage.IId, (void**)&obj);
            if (result.IsSuccess)
            {
                return new AudioMessageVst(this, obj, messageId);
            }

            throw new InvalidOperationException("Unable to create message");
        }

        public override void Dispose()
        {
            foreach (var stringToPtr in _managedToNativeUTF8)
            {
                NativeMemory.Free((void*)stringToPtr.Value);
            }

            _managedToNativeUTF8.Clear();
            _nativeUTF8ToManaged.Clear();

            NativeMemory.Free((void*)_tempBuffer);
        }

        public string GetOrCreateString(byte* str)
        {
            // var XxHash3.
            // calculate hashing
            var span = new Span<byte>(str, int.MaxValue);
            var index = span.IndexOf((byte)0);
            span = span.Slice(0, index);
            ulong hashing = 0UL;
            if (_nativeUTF8ToManaged.TryGetValue(hashing, out var managedString))
            {
                managedString = Encoding.UTF8.GetString(span);
                _nativeUTF8ToManaged.Add(hashing, managedString);
            }
            return managedString;
        }

        public FIDString GetOrCreateString(string str)
        {
            if (!_managedToNativeUTF8.TryGetValue(str, out var ptr))
            {
                var byteCount = Encoding.UTF8.GetByteCount(str);
                ptr = (IntPtr)NativeMemory.Alloc((nuint)(byteCount + 1));
                _managedToNativeUTF8.Add(str, ptr);
            }

            return new FIDString() { Value = (byte*)ptr };
        }
    }

    private class AudioMessageVst : AudioMessage
    {
        private readonly AudioHostApplicationVst _host;
        private readonly IMessage* _message;
        private readonly IAttributeList* _attributeList;

        public AudioMessageVst(AudioHostApplicationVst host, IMessage* message, string id) : base(id)
        {
            _host = host;
            _message = message;
            _attributeList = message->getAttributes();
            _message->setMessageID(host.GetOrCreateString(id));
        }

        public override bool TrySetInt64(string attributeId, long value)
        {
            return _attributeList->setInt(GetNativeAttributeId(attributeId), value);
        }

        public override bool TryGetInt64(string attributeId, out long value)
        {
            long localValue = 0;
            var result = _attributeList->getInt(GetNativeAttributeId(attributeId), &localValue);
            value = localValue;
            return result;
        }

        public override bool TrySetFloat64(string attributeId, double value)
        {
            return _attributeList->setFloat(GetNativeAttributeId(attributeId), value);
        }

        public override bool TryGetFloat64(string attributeId, out double value)
        {
            double localValue = 0;
            var result = _attributeList->getFloat(GetNativeAttributeId(attributeId), &localValue);
            value = localValue;
            return result;
        }

        public override bool TrySetString(string attributeId, string value)
        {
            fixed (char* pValue = value)
                return _attributeList->setString(GetNativeAttributeId(attributeId), pValue);
        }

        public override bool TryGetString(string attributeId, out string value)
        {
            value = string.Empty;
            var result = _attributeList->getString(GetNativeAttributeId(attributeId), (char*)_host.TempBuffer, AudioHostApplicationVst.TempBufferSize);
            if (result.IsSuccess)
            {
                var span = new ReadOnlySpan<char>((char*)_host.TempBuffer, int.MaxValue);
                span = span.Slice(0, span.IndexOf((char)0));
                value = new string(span);
            }
            return result;
        }

        public override bool TrySetBinary(string attributeId, ReadOnlySpan<byte> value)
        {
            fixed (byte* pBuffer = &MemoryMarshal.GetReference(value))
            {
                return _attributeList->setBinary(GetNativeAttributeId(attributeId), pBuffer, (uint)value.Length);
            }
        }

        public override bool TryGetBinary(string attributeId, out ReadOnlySpan<byte> value)
        {
            void* pBuffer;
            int size;
            var result = _attributeList->getBinary(GetNativeAttributeId(attributeId), &pBuffer, (uint*)&size);
            if (result)
            {
                value = new ReadOnlySpan<byte>(pBuffer, size);
                return true;
            }
            value = default;
            return false;
        }

        public override void Dispose()
        {
            _attributeList->release();
            _message->release();
        }
        private AttrID GetNativeAttributeId(string attributeId)
        {
            return new AttrID() { Value = _host.GetOrCreateString(attributeId).Value };
        }
    }
}