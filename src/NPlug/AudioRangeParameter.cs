// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Globalization;

namespace NPlug;

public sealed class AudioRangeParameter : AudioParameter
{
    public AudioRangeParameter(AudioParameterInfo info) : base(info)
    {
    }

    public AudioRangeParameter(string title, string? units = null, int id = 0, double minValue = 0.0, double maxValue = 1.0, double defaultPlainValue = 0.0, int stepCount = 0, AudioParameterFlags flags = AudioParameterFlags.CanAutomate, string? shortTitle = null) : base(title, units, id, shortTitle, stepCount, defaultPlainValue, flags)
    {
        if (minValue >= maxValue) throw new ArgumentException($"Invalid minimum, maximum. The minimum value {minValue} must be < to the maximum value {maxValue}");
        MinValue = minValue;
        MaxValue = maxValue;
        DefaultNormalizedValue = ToNormalized(defaultPlainValue);
        NormalizedValue = DefaultNormalizedValue;
    }

    public double MinValue { get; }

    public double MaxValue { get; }

    public double Value
    {
        get => ToPlain(RawNormalizedValue);
        set => RawNormalizedValue = ToNormalized(value);
    }

    public override string ToString(double valueNormalized)
    {
        if (StepCount > 1)
        {
            var value = (long)ToPlain(valueNormalized);
            return value.ToString(CultureInfo.InvariantCulture);
        }

        return base.ToString(ToPlain(valueNormalized));
    }

    public override double FromString(string plainValueAsString)
    {
        if (StepCount > 1)
        {
            long.TryParse(plainValueAsString, CultureInfo.InvariantCulture, out var value);
            return ToNormalized(value);
        }
        else
        {
            double.TryParse(plainValueAsString, CultureInfo.InvariantCulture, out var value);
            value = Math.Clamp(value, MinValue, MaxValue);
            return ToNormalized(value);
        }
    }

    public override double ToPlain(double normalizedValue)
    {
        var stepCount = StepCount;
        if (stepCount > 1)
        {
            return Math.Min(stepCount, (int)(normalizedValue * (stepCount + 1))) + MinValue;
        }

        return normalizedValue * (MaxValue - MinValue) + MinValue;
    }

    public override double ToNormalized(double plainValue)
    {
        var stepCount = StepCount;
        // Make sure that he value is within the range
        plainValue = Math.Clamp(plainValue, MinValue, MaxValue);
        if (stepCount > 1)
        {
            return (plainValue - MinValue) / stepCount;
        }

        return (plainValue - MinValue) / (MaxValue - MinValue);
    }
}