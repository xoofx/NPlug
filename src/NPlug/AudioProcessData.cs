// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug;

public readonly ref struct AudioProcessData
{
    private readonly IntPtr _context;

    public AudioProcessData(IntPtr context, AudioProcessMode processMode, AudioSampleSize sampleSize, int sampleCount, in AudioBusData input, in AudioBusData output)
    {
        _context = context;
        ProcessMode = processMode;
        SampleSize = sampleSize;
        SampleCount = sampleCount;
        Input = input;
        Output = output;
    }
    
    /// <summary>
    /// processing mode - value of @ref ProcessModes
    /// </summary>
    public readonly AudioProcessMode ProcessMode;

    /// <summary>
    /// sample size - value of @ref SymbolicSampleSizes
    /// </summary>
    public readonly AudioSampleSize SampleSize;

    /// <summary>
    /// number of samples to process
    /// </summary>
    public readonly int SampleCount;

    /// <summary>
    /// The input data.
    /// </summary>
    public readonly AudioBusData Input;

    /// <summary>
    /// The output data.
    /// </summary>
    public readonly AudioBusData Output;

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