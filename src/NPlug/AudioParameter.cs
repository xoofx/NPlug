// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

public class AudioParameter
{
    private double _normalizedValue;

    protected AudioParameterInfo InfoBase;

    public AudioParameter(AudioParameterInfo info)
    {
        Id = info.Id;
        InfoBase = info;
        Precision = 4;
        NormalizedValue = Info.DefaultNormalizedValue;
    }

    public AudioParameter(string title, int id = 0, string? units = null, string? shortTitle = null, int stepCount = 0, double defaultNormalizedValue = 0.0, AudioParameterFlags flags = AudioParameterFlags.CanAutomate)
    {
        Id = id;
        InfoBase = new AudioParameterInfo(id, title)
        {
            Units = units ?? string.Empty,
            ShortTitle = shortTitle ?? title,
            StepCount = stepCount,
            DefaultNormalizedValue = defaultNormalizedValue,
            Flags = flags
        };
        Precision = 4;
        NormalizedValue = defaultNormalizedValue;
    }

    public event Action? Changed;

    public bool IsControllerOnly { get; init; }

    public AudioParameterId Id { get; internal set; }

    public AudioParameterInfo Info => InfoBase with
    {
        Id = Id,
        UnitId = Unit?.Id ?? AudioUnitId.NoParent
    };
    
    public AudioUnit? Unit { get; internal set; }

    public double NormalizedValue
    {
        get => _normalizedValue;
        set
        {
            value = Math.Clamp(value, 0.0, 1.0);
            if (_normalizedValue != value)
            {
                _normalizedValue = value;
                Changed?.Invoke();
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
        if (Info.StepCount == 1)
        {
            return _normalizedValue > 0.5 ? "On" : "Off";
        }

        return valueNormalized.ToString(GetPrecisionFormat(Precision));
    }

    /// <summary>
    /// Convert a plain value as a string to a normalized value.
    /// </summary>
    /// <param name="plainValueAsString"></param>
    /// <returns></returns>
    public virtual double FromString(string plainValueAsString)
    {
        if (Info.StepCount == 1)
        {
            return (plainValueAsString.Equals("on", StringComparison.OrdinalIgnoreCase) || plainValueAsString.Equals("true", StringComparison.OrdinalIgnoreCase)) ? 1.0 : 0.0;
        }
        double.TryParse(plainValueAsString, out var value);
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
        return $"[{Info.Id.Value}] {Info.Title} = {ToString(NormalizedValue)}";
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