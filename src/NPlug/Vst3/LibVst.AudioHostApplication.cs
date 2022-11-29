// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    private sealed class AudioHostApplicationClient : AudioHostApplication
    {
        private readonly IHostApplication* _hostApplication;
        private readonly Dictionary<ulong, string> _nativeUTF8ToManaged;
        private readonly Dictionary<string, IntPtr> _managedToNativeUTF8;
        private readonly IntPtr _tempBuffer;
        private readonly Stack<AudioMessageClient> _audioMessagesCache;

        public AudioHostApplicationClient(IHostApplication* hostApplication, string name) : base(name)
        {
            _hostApplication = hostApplication;
            _nativeUTF8ToManaged = new Dictionary<ulong, string>();
            _managedToNativeUTF8 = new Dictionary<string, IntPtr>();
            _audioMessagesCache = new Stack<AudioMessageClient>();
            _tempBuffer = (IntPtr)NativeMemory.Alloc((nuint)TempBufferSize);
        }

        public IntPtr TempBuffer => _tempBuffer;

        public const int TempBufferSize = 4096;

        public override AudioMessage CreateMessage(string messageId)
        {
            IMessage* nativeMessage;
            var result = _hostApplication->createInstance(IMessage.NativeGuid, IMessage.NativeGuid, (void**)&nativeMessage);
            if (result.IsSuccess)
            {
                var message = _audioMessagesCache.Count > 0 ? _audioMessagesCache.Pop() : new AudioMessageClient(this);
                message.NativeMessage = nativeMessage;
                message.Id = messageId;
                return message;
            }

            throw new InvalidOperationException("Unable to create message");
        }

        internal void Return(AudioMessageClient message)
        {
            _audioMessagesCache.Push(message);
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
                // TODO: optimize memory allocation with a global allocator
                ptr = (IntPtr)NativeMemory.Alloc((nuint)(byteCount + 1));
                _managedToNativeUTF8.Add(str, ptr);
            }

            return new FIDString() { Value = (byte*)ptr };
        }
    }
}