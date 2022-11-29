// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    private class AudioMessageClient : AudioMessage
    {
        private readonly AudioHostApplicationClient _host;
        private IMessage* _message;
        private IAttributeList* _attributeList;

        public AudioMessageClient(AudioHostApplicationClient host)
        {
            _host = host;
        }

        public IMessage* NativeMessage
        {
            get => _message;
            set
            {
                _message = value;
                _attributeList = value != null ? value->getAttributes() : null;
            }
        }

        public override string Id
        {
            get
            {
                return _host.GetOrCreateString(_message->getMessageID().Value);
            }
            set
            {
                _message->setMessageID(_host.GetOrCreateString(value));
            }
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
            var result = _attributeList->getString(GetNativeAttributeId(attributeId), (char*)_host.TempBuffer, AudioHostApplicationClient.TempBufferSize);
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
            _attributeList = null;
            _message->release();
            _message = null;
            _host.Return(this);
        }

        private AttrID GetNativeAttributeId(string attributeId)
        {
            return new AttrID() { Value = _host.GetOrCreateString(attributeId).Value };
        }
    }
}