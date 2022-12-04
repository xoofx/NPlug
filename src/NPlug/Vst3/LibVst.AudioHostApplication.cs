// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NPlug.Backend;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    private sealed class AudioHostApplicationClient : AudioHostApplication, IAudioMessageBackend
    {
        private readonly IHostApplication* _hostApplication;
        private readonly Dictionary<ulong, string> _nativeUTF8ToManaged;
        private readonly Dictionary<string, IntPtr> _managedToNativeUTF8;

        public AudioHostApplicationClient(IHostApplication* hostApplication, string name) : base(name)
        {
            _hostApplication = hostApplication;
            _nativeUTF8ToManaged = new Dictionary<ulong, string>();
            _managedToNativeUTF8 = new Dictionary<string, IntPtr>();
        }

        public override bool TryCreateMessage(string messageId, out AudioMessage message)
        {
            IMessage* nativeMessage;
            var result = _hostApplication->createInstance(IMessage.NativeGuid, IMessage.NativeGuid, (void**)&nativeMessage);
            if (result.IsSuccess)
            {
                message = new AudioMessage(this, (IntPtr)nativeMessage, (IntPtr)nativeMessage->getAttributes());
                return true;
            }
            message = default;
            return false;
        }

        public override void Dispose()
        {
            foreach (var stringToPtr in _managedToNativeUTF8)
            {
                NativeMemory.Free((void*)stringToPtr.Value);
            }

            _managedToNativeUTF8.Clear();
            _nativeUTF8ToManaged.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetOrCreateString(byte* str)
        {
            var span = new Span<byte>(str, int.MaxValue);
            var index = span.IndexOf((byte)0);
            span = span.Slice(0, index);
            return GetOrCreateString(span);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetOrCreateString(ReadOnlySpan<byte> span)
        {
            var hashValue = XxHash3.HashToUInt64(span);
            lock (_nativeUTF8ToManaged)
            {
                ref var managedString = ref CollectionsMarshal.GetValueRefOrNullRef(_nativeUTF8ToManaged, hashValue);
                return managedString ??= Encoding.UTF8.GetString(span);
            }
        }

        public string GetOrCreateString128(char* pStr)
        {
            var span = new Span<char>(pStr, 128);
            var index = span.IndexOf((char)0);
            if (index >= 0)
            {
                span = span.Slice(0, index);
            }
            Span<byte> buffer = stackalloc byte[Encoding.UTF8.GetByteCount(span)];
            return GetOrCreateString(buffer);
        }

        public string GetOrCreateString(in String128 str)
        {
            fixed (char* pStr = str.Value)
            {
                return GetOrCreateString128(pStr);
            }
        }

        public FIDString GetOrCreateString(string str)
        {
            lock (_managedToNativeUTF8)
            {
                ref var ptr = ref CollectionsMarshal.GetValueRefOrNullRef(_managedToNativeUTF8, str);
                var localPtr = ptr;
                if (localPtr == IntPtr.Zero)
                {
                    var byteCount = Encoding.UTF8.GetByteCount(str);
                    // TODO: optimize memory allocation with a global allocator
                    localPtr = (IntPtr)NativeMemory.Alloc((nuint)(byteCount + 1));
                    _managedToNativeUTF8.Add(str, localPtr);
                    ptr = localPtr;
                }
                return new FIDString() { Value = (byte*)localPtr };
            }
        }

        string IAudioMessageBackend.GetId(in AudioMessage message)
        {
            return GetOrCreateString(((IMessage*)message.MessageContext)->getMessageID().Value);
        }

        void IAudioMessageBackend.SetId(in AudioMessage message, string id)
        {
            ((IMessage*)message.MessageContext)->setMessageID(GetOrCreateString(id));
        }

        bool IAudioMessageBackend.TrySetInt64(in AudioMessage message, string attributeId, long value)
        {
            return ((IAttributeList*)message.AttributeContext)->setInt(GetNativeAttributeId(attributeId), value);
        }

        bool IAudioMessageBackend.TryGetInt64(in AudioMessage message, string attributeId, out long value)
        {
            long localValue = 0;
            var result = ((IAttributeList*)message.AttributeContext)->getInt(GetNativeAttributeId(attributeId), &localValue);
            value = localValue;
            return result;
        }

        bool IAudioMessageBackend.TrySetFloat64(in AudioMessage message, string attributeId, double value)
        {
            return ((IAttributeList*)message.AttributeContext)->setFloat(GetNativeAttributeId(attributeId), value);
        }

        bool IAudioMessageBackend.TryGetFloat64(in AudioMessage message, string attributeId, out double value)
        {
            double localValue = 0;
            var result = ((IAttributeList*)message.AttributeContext)->getFloat(GetNativeAttributeId(attributeId), &localValue);
            value = localValue;
            return result;
        }

        bool IAudioMessageBackend.TrySetString(in AudioMessage message, string attributeId, string value)
        {
            fixed (char* pValue = value)
                return ((IAttributeList*)message.AttributeContext)->setString(GetNativeAttributeId(attributeId), pValue);
        }

        bool IAudioMessageBackend.TryGetString(in AudioMessage message, string attributeId, out string value)
        {
            value = string.Empty;
            const int TempBufferSize = 4096;
            var array = ArrayPool<byte>.Shared.Rent(TempBufferSize);
            try
            {
                fixed (byte* pValue = array)
                {
                    var result = ((IAttributeList*)message.AttributeContext)->getString(GetNativeAttributeId(attributeId), (char*)pValue, TempBufferSize);
                    if (result.IsSuccess)
                    {
                        var span = new ReadOnlySpan<char>((char*)pValue, int.MaxValue);
                        span = span.Slice(0, span.IndexOf((char)0));
                        value = new string(span);
                        return true;
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }

            return false;
        }

        bool IAudioMessageBackend.TrySetBinary(in AudioMessage message, string attributeId, ReadOnlySpan<byte> value)
        {
            fixed (byte* pBuffer = &MemoryMarshal.GetReference(value))
            {
                return ((IAttributeList*)message.AttributeContext)->setBinary(GetNativeAttributeId(attributeId), pBuffer, (uint)value.Length);
            }
        }

        bool IAudioMessageBackend.TryGetBinary(in AudioMessage message, string attributeId, out ReadOnlySpan<byte> value)
        {
            void* pBuffer;
            int size;
            var result = ((IAttributeList*)message.AttributeContext)->getBinary(GetNativeAttributeId(attributeId), &pBuffer, (uint*)&size);
            if (result)
            {
                value = new ReadOnlySpan<byte>(pBuffer, size);
                return true;
            }
            value = default;
            return false;
        }

        void IAudioMessageBackend.Destroy(in AudioMessage message)
        {
            ((IMessage*)message.MessageContext)->release();
            ((IAttributeList*)message.AttributeContext)->release();
        }

        private AttrID GetNativeAttributeId(string attributeId)
        {
            return new AttrID() { Value = GetOrCreateString(attributeId).Value };
        }
    }
}