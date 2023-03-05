// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NPlug.Proxy;

internal sealed class AudioPluginProxy : IDisposable
{
    private readonly IntPtr _nativeProxyHandle;
    private readonly bool _ownHandle;
    private Func<IntPtr>? _nativeFactory;
    private readonly InternalGetFactoryDelegate _internalGetFactoryDelegate;
    private GCHandle _internalGetFactoryDelegateHandle;
    private bool _disposed;

    private AudioPluginProxy(IntPtr nativeProxyHandle, bool ownHandle)
    {
        _nativeProxyHandle = nativeProxyHandle;
        _ownHandle = ownHandle;

        var nativeProxySetFactory = NativeLibrary.GetExport(nativeProxyHandle, "nplug_set_plugin_factory");
        if (nativeProxySetFactory == IntPtr.Zero)
        {
            throw new InvalidOperationException("Missing nplug_set_plugin_factory from the proxy library");
        }

        _internalGetFactoryDelegate = InternalGetFactory;
        _internalGetFactoryDelegateHandle = GCHandle.Alloc(_internalGetFactoryDelegate, GCHandleType.Normal);
        _nativeFactory = null;
        // Setup the proxy immediately
        unsafe
        {
            ((delegate*unmanaged<void*, void>)nativeProxySetFactory)((void*)Marshal.GetFunctionPointerForDelegate(_internalGetFactoryDelegate));
        }
    }

    public void SetNativeFactory(Func<IntPtr> nativeFactory)
    {
        _nativeFactory = nativeFactory;
    }

    public static string GetVstArchitecture()
    {
        if (OperatingSystem.IsWindows())
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {

                case Architecture.Arm64:
                    return "arm_64-win";
                case Architecture.X64:
                    return "x86_64-win";
                default:
                    throw new PlatformNotSupportedException($"Processor {RuntimeInformation.ProcessArchitecture} not supported for the Windows platform");
            }
        }

        if (OperatingSystem.IsMacOS())
        {
            return "MacOS";
        }

        if (OperatingSystem.IsLinux())
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {

                case Architecture.Arm64:
                    return "arch64-linux";
                case Architecture.X64:
                    return "x86_64-linux";
                default:
                    throw new PlatformNotSupportedException($"Processor {RuntimeInformation.ProcessArchitecture} not supported for the Linux platform");
            }
        }

        throw new PlatformNotSupportedException();
    }

    public static string GetVstDynamicLibraryName(string name)
    {
        if (OperatingSystem.IsWindows())
        {
            return $"{name}.vst3";
        }

        if (OperatingSystem.IsMacOS())
        {
            return $"{name}.dylib";
        }

        if (OperatingSystem.IsLinux())
        {
            return $"{name}.so";
        }

        throw new PlatformNotSupportedException();
    }

    public static AudioPluginProxy Load(string nativeProxyDllFilePath)
    {
        if (!File.Exists(nativeProxyDllFilePath))
        {
            throw new FileNotFoundException(nameof(nativeProxyDllFilePath));
        }
        var nativeImport = NativeLibrary.Load(nativeProxyDllFilePath);
        if (nativeImport != IntPtr.Zero)
        {
            return new AudioPluginProxy(nativeImport, true);
        }

        throw new InvalidOperationException($"Unable to load native proxy from path {nativeProxyDllFilePath}");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _internalGetFactoryDelegateHandle.Free();
        if (_ownHandle)
        {
            NativeLibrary.Free(_nativeProxyHandle);
        }
    }
    
    private IntPtr InternalGetFactory()
    {
        return _nativeFactory?.Invoke() ?? IntPtr.Zero;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr InternalGetFactoryDelegate();
}