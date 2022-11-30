// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace NPlug.Vst3;

internal static partial class LibVst
{
    /// <summary>
    /// A particular interface implementation linking to a <see cref="ComObject"/>
    /// </summary>
    public unsafe struct ComObjectHandle
    {
        public void** Vtbl;
        public Guid Guid;
        public GCHandle Handle;

        public ComObject ComObject => (ComObject)Handle.Target!;

        public object? Target => ((ComObject)Handle.Target!).Target;
    }

    /// <summary>
    /// ComObject bridges many COM interfaces to a single C# implementation.
    /// </summary>
    public sealed unsafe class ComObject : IDisposable
    {
        private const int MaxInterfacesPerObject = 32;
        private uint _refCount;
        private SpinLock _lock;
        private readonly ComObjectHandle* _handles;
        private int _count;
        private GCHandle _thisHandle;

        public ComObject(ComObjectManager manager)
        {
            Manager = manager;
            _handles = (ComObjectHandle*)NativeMemory.AllocZeroed((nuint)(sizeof(ComObjectHandle) * MaxInterfacesPerObject));
            _thisHandle = GCHandle.Alloc(this);
        }

        public ComObjectManager Manager { get; }

        public object? Target { get; set; }

        public uint AddRef()
        {
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);
                _refCount++;
                return _refCount;
            }
            finally
            {
                if (lockTaken) _lock.Exit();
            }
        }

        public uint ReleaseRef()
        {
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);
                if (_refCount > 0)
                {
                    _refCount--;
                    if (_refCount == 0 && Target is IDisposable disposable)
                    {
                        disposable.Dispose();
                        Reset();
                    }
                }
                return _refCount;
            }
            finally
            {
                if (lockTaken) _lock.Exit();
            }
        }

        public ComObjectHandle* GetOrComObjectHandle<T>() where T: INativeGuid, INativeVtbl
        {
            ref var guid = ref *T.NativeGuid;
            for (int i = 0; i < _count; i++)
            {
                var handle = _handles + i;
                // TODO: optimize guid comparison (it is making a copy on the stack, instead of a fast byte by byte comparer)
                if (handle->Guid == guid)
                {
                    return handle;
                }
            }

            if (_count == MaxInterfacesPerObject)
            {
                throw new InvalidOperationException($"Cannot create more handle for a COM object. Maximum of {MaxInterfacesPerObject} interfaces has been reached.");
            }

            var nextHandle = _handles + _count;
            _count++;

            nextHandle->Vtbl = VtblInitializer<T>.Vtbl;
            nextHandle->Guid = guid;
            nextHandle->Handle = _thisHandle;
            return nextHandle;
        }

        public void Reset()
        {
            for (int i = 0; i < _count; i++)
            {
                _handles[i] = default;
            }
            _refCount = 0;
            _lock = default;
            Target = null;
        }

        private static class VtblInitializer<T> where T: INativeVtbl
        {
            public static readonly void** Vtbl;

            static VtblInitializer()
            {
                Vtbl = (void**)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(T), T.VtblCount * IntPtr.Size);
                T.InitializeVtbl(Vtbl);
            }
        }

        public void Dispose()
        {
            NativeMemory.Free(_handles);
            _thisHandle.Free();
        }
    }

    public sealed class ComObjectManager : IDisposable
    {
        private const int DefaultComObjectCacheCount = 16;
        private readonly Stack<ComObject> _comObjectCache;
        private SpinLock _lock;

        public ComObjectManager()
        {
            _comObjectCache = new Stack<ComObject>();
            for (int i = 0; i < DefaultComObjectCacheCount; i++)
            {
                _comObjectCache.Push(new ComObject(this));
            }
        }

        public ComObject GetOrCreateComObject()
        {
            bool taken = false;
            try
            {
                _lock.Enter(ref taken);
                if (_comObjectCache.Count > 0) return _comObjectCache.Pop();
            }
            finally
            {
                if (taken) _lock.Exit();
            }
            return new ComObject(this);
        }

        public void Dispose()
        {
        }
    }


    public unsafe interface INativeGuid
    {
        public static abstract Guid* NativeGuid { get; }
    }

    public unsafe interface INativeVtbl
    {
        public static abstract int VtblCount { get; }

        public static abstract void InitializeVtbl(void** vtbl);
    }
}