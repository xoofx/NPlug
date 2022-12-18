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

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    private sealed class AudioHostApplicationClient : AudioHostApplication, IAudioMessageBackend, IAudioAttributeListBackend
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
                message = new AudioMessage(this, (IntPtr)nativeMessage, new AudioAttributeList(this, (IntPtr)nativeMessage->getAttributes()));
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
                ref var managedString = ref CollectionsMarshal.GetValueRefOrAddDefault(_nativeUTF8ToManaged, hashValue, out _);
                return managedString ??= Encoding.UTF8.GetString(span);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetOrCreateString(byte* str, int maxLength)
        {
            var span = new ReadOnlySpan<byte>(str, maxLength);
            var indexOfZero = span.IndexOf((byte)0);
            if (indexOfZero >= 0)
            {
                span = span.Slice(0, indexOfZero);
            }
            return GetOrCreateString(span);
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
                ref var ptr = ref CollectionsMarshal.GetValueRefOrAddDefault(_managedToNativeUTF8, str, out _);
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
            return GetOrCreateString(((IMessage*)message.NativeContext)->getMessageID());
        }

        void IAudioMessageBackend.SetId(in AudioMessage message, string id)
        {
            ((IMessage*)message.NativeContext)->setMessageID(GetOrCreateString(id));
        }

        bool IAudioAttributeListBackend.TrySetInt64(in AudioAttributeList attributeList, string attributeId, long value)
        {
            return ((IAttributeList*)attributeList.NativeContext)->setInt(GetNativeAttributeId(attributeId), value);
        }

        bool IAudioAttributeListBackend.TryGetInt64(in AudioAttributeList attributeList, string attributeId, out long value)
        {
            long localValue = 0;
            var result = ((IAttributeList*)attributeList.NativeContext)->getInt(GetNativeAttributeId(attributeId), &localValue);
            value = localValue;
            return result;
        }

        bool IAudioAttributeListBackend.TrySetFloat64(in AudioAttributeList attributeList, string attributeId, double value)
        {
            return ((IAttributeList*)attributeList.NativeContext)->setFloat(GetNativeAttributeId(attributeId), value);
        }

        bool IAudioAttributeListBackend.TryGetFloat64(in AudioAttributeList attributeList, string attributeId, out double value)
        {
            double localValue = 0;
            var result = ((IAttributeList*)attributeList.NativeContext)->getFloat(GetNativeAttributeId(attributeId), &localValue);
            value = localValue;
            return result;
        }

        bool IAudioAttributeListBackend.TrySetString(in AudioAttributeList attributeList, string attributeId, string value)
        {
            fixed (char* pValue = value)
                return ((IAttributeList*)attributeList.NativeContext)->setString(GetNativeAttributeId(attributeId), pValue);
        }

        bool IAudioAttributeListBackend.TryGetString(in AudioAttributeList attributeList, string attributeId, out string value)
        {
            value = string.Empty;
            const int TempBufferSize = 4096;
            var array = ArrayPool<byte>.Shared.Rent(TempBufferSize);
            try
            {
                fixed (byte* pValue = array)
                {
                    var result = ((IAttributeList*)attributeList.NativeContext)->getString(GetNativeAttributeId(attributeId), (char*)pValue, TempBufferSize);
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

        bool IAudioAttributeListBackend.TrySetBinary(in AudioAttributeList attributeList, string attributeId, ReadOnlySpan<byte> value)
        {
            fixed (byte* pBuffer = &MemoryMarshal.GetReference(value))
            {
                return ((IAttributeList*)attributeList.NativeContext)->setBinary(GetNativeAttributeId(attributeId), pBuffer, (uint)value.Length);
            }
        }

        bool IAudioAttributeListBackend.TryGetBinary(in AudioAttributeList attributeList, string attributeId, out ReadOnlySpan<byte> value)
        {
            void* pBuffer;
            int size;
            var result = ((IAttributeList*)attributeList.NativeContext)->getBinary(GetNativeAttributeId(attributeId), &pBuffer, (uint*)&size);
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
            ((IMessage*)message.NativeContext)->release();
            ((IAttributeList*)message.AttributeList.NativeContext)->release();
        }

        private AttrID GetNativeAttributeId(string attributeId)
        {
            return new AttrID() { Value = GetOrCreateString(attributeId).Value };
        }
    }
}