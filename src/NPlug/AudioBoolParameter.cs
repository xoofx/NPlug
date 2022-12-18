// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Globalization;

namespace NPlug;

public class AudioBoolParameter : AudioParameter
{
    public AudioBoolParameter(AudioParameterInfo info) : base(info)
    {
        StepCount = 1;
    }

    public AudioBoolParameter(string title, int id = 0, string? units = null, string? shortTitle = null, bool defaultValue = false, AudioParameterFlags flags = AudioParameterFlags.CanAutomate) : base(title, id, units, shortTitle, 1, defaultValue ? 1.0 : 0.0, flags)
    {
    }

    public bool Value
    {
        get
        {
            return NormalizedValue > 0.5;
        }
        set
        {
            NormalizedValue = value ? 1.0 : 0.0;
        }
    }
}

public sealed class AudioRangeParameter : AudioParameter
{
    public AudioRangeParameter(AudioParameterInfo info) : base(info)
    {
    }

    public AudioRangeParameter(string title, int id = 0, string? units = null, double minPlainValue = 0.0, double maxPlainValue = 1.0, double defaultPlainValue = 0.0, int stepCount = 0, AudioParameterFlags flags = AudioParameterFlags.CanAutomate, string? shortTitle = null) : base(title, id, units, shortTitle, stepCount, defaultPlainValue, flags)
    {
        MinPlainValue = minPlainValue;
        MaxPlainValue = maxPlainValue;
        DefaultNormalizedValue = ToNormalized(defaultPlainValue);
        NormalizedValue = DefaultNormalizedValue;
    }

    public double MinPlainValue { get; set; }

    public double MaxPlainValue { get; set; }

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
            long.TryParse(plainValueAsString, out var value);
            return ToNormalized(value);
        }
        else
        {
            double.TryParse(plainValueAsString, out var value);
            value = Math.Clamp(value, MinPlainValue, MaxPlainValue);
            return ToNormalized(value);
        }
    }

    public override double ToPlain(double normalizedValue)
    {
        var stepCount = StepCount;
        if (stepCount > 1)
        {
            return Math.Min(stepCount, (int)(normalizedValue * (stepCount + 1))) + MinPlainValue;
        }

        return normalizedValue * (MaxPlainValue - MinPlainValue) + MinPlainValue;
    }

    public override double ToNormalized(double plainValue)
    {
        var stepCount = StepCount;
        if (stepCount > 1)
        {
            return (plainValue - MinPlainValue) / stepCount;
        }

        return (plainValue - MinPlainValue) / (MaxPlainValue - MinPlainValue);
    }
}

public class AudioStringListParameter : AudioParameter
{
    private string[] _items;

    public AudioStringListParameter(AudioParameterInfo info) : base(info)
    {
        var items = Array.Empty<string>();
        _items = items;
        Items = items;
        DefaultNormalizedValue = 0.0;
        NormalizedValue = 0.0;
    }

    public AudioStringListParameter(string title, string[] items, int id = 0, string? units = null, string? shortTitle = null, int selectedItem = 0, AudioParameterFlags flags = AudioParameterFlags.CanAutomate | AudioParameterFlags.IsList) : base(title, id, units, shortTitle, 0, 0.0, flags)
    {
        _items = items;
        Items = items;
        DefaultNormalizedValue = ToNormalized(selectedItem);
    }
    
    public string[] Items
    {
        get => _items;
        set
        {
            if (value.Length < 2) throw new ArgumentException("Expecting an array with at least 2 strings");
            _items = value;
            StepCount = _items.Length - 1;
            DefaultNormalizedValue = 0.0;
        }
    }

    public int SelectedItem
    {
        get
        {
            return (int)ToPlain(RawNormalizedValue);
        }
        set
        {
            if ((uint)value >= (uint)Items.Length) throw new ArgumentOutOfRangeException(nameof(value));
            NormalizedValue = ToNormalized(value);
        }
    }

    public override string ToString(double valueNormalized)
    {
        if (StepCount < 1) return string.Empty;

        return Items[(int)ToPlain(valueNormalized)];
    }

    public override double FromString(string plainValueAsString)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            var item = _items[i];
            if (item.Equals(plainValueAsString, StringComparison.Ordinal))
            {
                return ToNormalized(i);
            }
        }

        return 0.0;
    }

    public sealed override double ToPlain(double normalizedValue)
    {
        var stepCount = StepCount;
        if (stepCount < 1) return 0.0;

        return (int)(Math.Min(stepCount, normalizedValue * (stepCount + 1)));
    }

    public sealed override double ToNormalized(double plainValue)
    {
        var stepCount = StepCount;
        if (stepCount < 1) return 0.0;

        return plainValue / stepCount;
    }
}
