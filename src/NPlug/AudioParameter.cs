// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace NPlug;

public class AudioParameter
{
    /// <summary>
    /// This value is only used temporarily until the <see cref="AudioProcessorModel"/> is initialized.
    /// Then it is the <see cref="PointerToNormalizedValueInSharedBuffer"/> that is used.
    /// </summary>
    internal double LocalNormalizedValue;

    /// <summary>
    /// This pointer is setup by <see cref="AudioProcessorModel"/> in InitializeBuffers.
    /// </summary>
    internal unsafe double* PointerToNormalizedValueInSharedBuffer;

    public AudioParameter(AudioParameterInfo info)
    {
        Id = info.Id;
        Title = info.Title;
        ShortTitle = info.ShortTitle;
        Units = info.Units;
        StepCount = info.StepCount;
        DefaultNormalizedValue = Math.Clamp(info.DefaultNormalizedValue, 0.0, 1.0);
        Flags = info.Flags;
        Precision = 4;
        NormalizedValue = DefaultNormalizedValue;
    }

    public AudioParameter(string title, string? units = null, int id = 0, string? shortTitle = null, int stepCount = 0, double defaultNormalizedValue = 0.0, AudioParameterFlags flags = AudioParameterFlags.CanAutomate)
    {
        Id = id;
        Title = title;
        ShortTitle = shortTitle ?? title;
        Units = units ?? string.Empty;
        StepCount = stepCount;
        DefaultNormalizedValue = Math.Clamp(defaultNormalizedValue, 0.0, 1.0);
        Flags = flags;
        Precision = 4;
        NormalizedValue = DefaultNormalizedValue;
    }

    public bool IsControllerOnly { get; init; }

    /// <summary>
    /// unique identifier of this parameter (named tag too)
    /// </summary>
    public AudioParameterId Id { get; internal set; }

    /// <summary>
    /// parameter title (e.g. "Volume")
    /// </summary>
    /// <remarks>
    /// Changing this property after registration requires to call <see cref="IAudioControllerHandler.RestartComponent"/> with <see cref="AudioRestartFlags.ParamTitlesChanged"/>.
    /// </remarks>
    public string Title { get; set; }

    /// <summary>
    /// parameter shortTitle (e.g. "Vol")
    /// </summary>
    /// <remarks>
    /// Changing this property after registration requires to call <see cref="IAudioControllerHandler.RestartComponent"/> with <see cref="AudioRestartFlags.ParamTitlesChanged"/>.
    /// </remarks>
    public string ShortTitle { get; set; }

    /// <summary>
    /// parameter unit (e.g. "dB")
    /// </summary>
    /// <remarks>
    /// Changing this property after registration requires to call <see cref="IAudioControllerHandler.RestartComponent"/> with <see cref="AudioRestartFlags.ParamTitlesChanged"/>.
    /// </remarks>
    public string Units { get; set; }

    /// <summary>
    /// number of discrete steps (0: continuous, 1: toggle, discrete value otherwise 
    /// (corresponding to max - min, for example: 127 for a min = 0 and a max = 127) - see @ref vst3ParameterIntro)
    /// </summary>
    public int StepCount { get; protected set; }

    /// <summary>
    /// default normalized value [0,1] (in case of discrete value: defaultNormalizedValue = defDiscreteValue / stepCount)
    /// </summary>
    /// <remarks>
    /// Changing this property after registration requires to call <see cref="IAudioControllerHandler.RestartComponent"/> with <see cref="AudioRestartFlags.ParamTitlesChanged"/>.
    /// </remarks>
    public double DefaultNormalizedValue { get; set; }

    /// <summary>
    /// id of unit this parameter belongs to (see @ref vst3Units)
    /// </summary>
    public AudioUnit? Unit { get; internal set; }

    /// <summary>
    /// Flags for the parameter
    /// </summary>
    /// <remarks>
    /// Changing this property after registration requires to call <see cref="IAudioControllerHandler.RestartComponent"/> with <see cref="AudioRestartFlags.ParamTitlesChanged"/>.
    /// </remarks>
    public AudioParameterFlags Flags { get; set; }

    /// <summary>
    /// Gets a boolean indicating whether this parameter is a program change parameter.
    /// </summary>
    public bool IsProgramChange => (Flags & AudioParameterFlags.IsProgramChange) != 0;
    
    /// <summary>
    /// Gets the associated <see cref="AudioParameterInfo"/> object.
    /// </summary>
    /// <returns></returns>
    public AudioParameterInfo GetInfo() => new (Id, Title)
    {
        ShortTitle = ShortTitle,
        Units = Units,
        StepCount = StepCount,
        DefaultNormalizedValue = DefaultNormalizedValue,
        UnitId = Unit?.Id ?? AudioUnitId.NoParent,
        Flags = Flags,
    };

    /// <summary>
    /// Gets a direct access to the raw normalized value.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="NormalizedValue"/>, this doesn't clamp or trigger a <see cref="Changed"/> event.
    /// </remarks>
    internal unsafe ref double RawNormalizedValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (PointerToNormalizedValueInSharedBuffer != null)
            { 
                return ref *PointerToNormalizedValueInSharedBuffer;
            }
            else
            {
                return ref LocalNormalizedValue;
            }
        }
    }

    public double NormalizedValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => RawNormalizedValue;
        set
        {
            value = Math.Clamp(value, 0.0, 1.0);
            ref var rawNormalizedValue = ref RawNormalizedValue;
            if (rawNormalizedValue != value)
            {
                rawNormalizedValue = value;
                OnParameterValueChanged();
            }
        }
    }

    public int Precision { get; set; }

    /// <summary>
    /// Convert a normalized value to a plain value as a string.
    /// </summary>
    /// <param name="valueNormalized"></param>
    /// <returns></returns>
    public virtual string ToString(double valueNormalized)
    {
        if (StepCount == 1)
        {
            return RawNormalizedValue > 0.5 ? "On" : "Off";
        }
        return valueNormalized.ToString(GetPrecisionFormat(Precision), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Convert a plain value as a string to a normalized value.
    /// </summary>
    /// <param name="plainValueAsString"></param>
    /// <returns></returns>
    public virtual double FromString(string plainValueAsString)
    {
        if (StepCount == 1)
        {
            return (plainValueAsString.Equals("on", StringComparison.OrdinalIgnoreCase) || plainValueAsString.Equals("true", StringComparison.OrdinalIgnoreCase)) ? 1.0 : 0.0;
        }
        double.TryParse(plainValueAsString, CultureInfo.InvariantCulture, out var value);
        return value;
    }

    public virtual double ToPlain(double normalizedValue)
    {
        return normalizedValue;
    }

    public virtual double ToNormalized(double plainValue)
    {
        return plainValue;
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Title} = {ToString(NormalizedValue)} {Units}";
    }

    private void OnParameterValueChanged()
    {
        Unit?.OnParameterValueChangedInternal(this);
    }

    private static string GetPrecisionFormat(int precision)
    {
        return precision switch
        {
            0 => "N0",
            1 => "N1",
            2 => "N2",
            3 => "N3",
            4 => "N4",
            5 => "N5",
            6 => "N6",
            7 => "N7",
            8 => "N8",
            9 => "N9",
            10 => "N10",
            11 => "N11",
            12 => "N12",
            13 => "N13",
            14 => "N14",
            15 => "N15",
            16 => "N16",
            _ => "N4",
        };
    }
}