// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NPlug;

public ref struct AudioProcessData
{
    private readonly IntPtr _context;

    internal AudioProcessData(IntPtr context)
    {
        _context = context;
    }

    /// <summary>
    /// processing mode - value of @ref ProcessModes
    /// </summary>
    public AudioProcessMode ProcessMode;

    /// <summary>
    /// sample size - value of @ref SymbolicSampleSizes
    /// </summary>
    public AudioSampleSize SampleSize;

    /// <summary>
    /// number of samples to process
    /// </summary>
    public int SampleCount;

    /// <summary>
    /// The input data.
    /// </summary>
    public AudioBusData Input;

    /// <summary>
    /// The output data.
    /// </summary>
    public AudioBusData Output;

    /// <summary>
    /// Gets a boolean indicating if <see cref="GetContext"/> will return a value.
    /// </summary>
    public bool HasContext => _context != IntPtr.Zero;

    /// <summary>
    /// processing context (optional, but most welcome)
    /// </summary>
    public ref AudioProcessContext GetContext()
    {
        if (!HasContext) throw new InvalidOperationException("No context is available. Check HasContext before accessing this method");
        unsafe
        {
            return ref Unsafe.AsRef<AudioProcessContext>((void*)_context);
        }
    }
}