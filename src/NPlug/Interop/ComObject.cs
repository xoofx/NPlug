// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace NPlug.Interop;

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

        public T As<T>() where T : class => (T)Target!;
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
        private int _interfaceCount;
        private GCHandle _thisHandle;

        public ComObject(ComObjectManager manager)
        {
            Manager = manager;
            _handles = (ComObjectHandle*)NativeMemory.AllocZeroed((nuint)(sizeof(ComObjectHandle) * MaxInterfacesPerObject));
            _thisHandle = GCHandle.Alloc(this);
            _refCount = 0;
        }

        public ComObjectManager Manager { get; }

        public int InterfaceCount => _interfaceCount;

        public uint ReferenceCount => _refCount;

        public IntPtr GetInterfacePointer(int index, out Guid guid)
        {
            if ((uint)index >= (uint)_interfaceCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var ptr = _handles + index;
            guid = ptr->Guid;
            return (IntPtr)ptr;
        }
        
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
            bool needToReturn = false;
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);
                if (_refCount > 0)
                {
                    _refCount--;
                    if (_refCount == 0)
                    {
                        if (Target is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                        needToReturn = true;
                    }
                }
                return _refCount;
            }
            finally
            {
                if (lockTaken) _lock.Exit();
                if (needToReturn)
                {
                    Reset();
                    Manager.Return(this);
                }
            }
        }

        public T* QueryInterface<T>() where T: unmanaged, INativeGuid, INativeVtbl
        {
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);
                _refCount++;

                var guidToFind = *T.NativeGuid;
                for (int i = 0; i < _interfaceCount; i++)
                {
                    var handle = _handles + i;
                    if (handle->Guid.Equals(guidToFind))
                    {
                        return (T*)handle;
                    }
                }

                if (_interfaceCount == MaxInterfacesPerObject)
                {
                    _refCount--;
                    throw new InvalidOperationException($"Cannot create more handle for a COM object. Maximum of {MaxInterfacesPerObject} interfaces has been reached.");
                }

                var nextHandle = _handles + _interfaceCount;
                _interfaceCount++;

                nextHandle->Vtbl = (void**)ComObjectManager.GetVtbl<T>();
                nextHandle->Guid = guidToFind;
                nextHandle->Handle = _thisHandle;
                return (T*)nextHandle;
            }
            finally
            {
                if (lockTaken) _lock.Exit();
            }
        }

        public IntPtr QueryInterface(in Guid guidToFind)
        {
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);
                _refCount++;


                for (int i = 0; i < _interfaceCount; i++)
                {
                    var handle = _handles + i;
                    if (handle->Guid.Equals(guidToFind))
                    {
                        return (IntPtr)handle;
                    }
                }

                if (_interfaceCount == MaxInterfacesPerObject)
                {
                    _refCount--;
                    throw new InvalidOperationException($"Cannot create more handle for a COM object. Maximum of {MaxInterfacesPerObject} interfaces has been reached.");
                }

                var nextHandle = _handles + _interfaceCount;
                _interfaceCount++;

                nextHandle->Vtbl = (void**)ComObjectManager.GetVtbl(guidToFind);
                nextHandle->Guid = guidToFind;
                nextHandle->Handle = _thisHandle;
                return (IntPtr)nextHandle;
            }
            finally
            {
                if (lockTaken) _lock.Exit();
            }
        }

        public void Reset()
        {
            for (int i = 0; i < _interfaceCount; i++)
            {
                _handles[i] = default;
            }

            _interfaceCount = 0;
            _refCount = 0;
            _lock = default;
        }

        public void Dispose()
        {
            NativeMemory.Free(_handles);
            _thisHandle.Free();
        }
    }

    public sealed unsafe partial class ComObjectManager : IDisposable
    {
        private static readonly Dictionary<Guid, Func<IntPtr>> MapGuidToVtblFunc = new Dictionary<Guid, Func<IntPtr>>();

        public static readonly ComObjectManager Instance = new ComObjectManager();
        
        private const int DefaultComObjectCacheCount = 16;
        private readonly Stack<ComObject> _comObjectCache;
        private readonly Dictionary<object, ComObject> _mapTargetToComObject;
        private readonly object _thisLock;

        public ComObjectManager()
        {
            _comObjectCache = new Stack<ComObject>();
            for (int i = 0; i < DefaultComObjectCacheCount; i++)
            {
                _comObjectCache.Push(new ComObject(this));
            }
            _mapTargetToComObject = new Dictionary<object, ComObject>(ReferenceEqualityComparer.Instance);
            _thisLock = new object();
        }

        public ComObject GetOrCreateComObject(object target)
        {
            lock (_thisLock)
            {
                if (_mapTargetToComObject.TryGetValue(target, out var comObject))
                {
                    return comObject;
                }

                comObject = _comObjectCache.Count > 0 ? _comObjectCache.Pop() : new ComObject(this);
                _mapTargetToComObject.Add(target, comObject);
                comObject.Target = target;
                return comObject;
            }
        }

        public ComObject[] GetAliveComObjects()
        {
            lock (_thisLock)
            {
                return _mapTargetToComObject.Values.ToArray();
            }
        }

        public void Return(ComObject comObject)
        {
            var target = comObject.Target!;
            comObject.Target = null;
            lock (_thisLock)
            {
                _mapTargetToComObject.Remove(target);
                _comObjectCache.Push(comObject);
            }
        }

        public void Dispose()
        {
            lock (_thisLock)
            {
                foreach (var comObject in _comObjectCache)
                {
                    comObject.Dispose();
                }

                _comObjectCache.Clear();
                _mapTargetToComObject.Clear();
            }
        }

        private static void Register<T>() where T : struct, INativeGuid, INativeVtbl
        {
            MapGuidToVtblFunc[*(T.NativeGuid)] = () => (IntPtr)VtblInitializer<T>.Vtbl;
        }

        public static IntPtr GetVtbl<T>() where T : struct, INativeGuid, INativeVtbl
        {
            return (IntPtr)VtblInitializer<T>.Vtbl;
        }

        public static IntPtr GetVtbl(in Guid guid)
        {
            if (MapGuidToVtblFunc.TryGetValue(guid, out var vtbFuncl))
            {
                return vtbFuncl();
            }
            return IntPtr.Zero;
        }

        private static class VtblInitializer<T> where T : INativeVtbl
        {
            public static readonly void** Vtbl;

            static VtblInitializer()
            {
                Vtbl = (void**)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(T), T.VtblCount * IntPtr.Size);
                T.InitializeVtbl(Vtbl);
            }
        }

        static ComObjectManager()
        {
            RegisterAllInterfaces();
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