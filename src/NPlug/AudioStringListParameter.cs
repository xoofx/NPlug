// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

public class AudioStringListParameter : AudioParameter
{
    private readonly string[] _items;

    public AudioStringListParameter(AudioParameterInfo info, string[] items) : base(info)
    {
        if (items.Length < 2) throw new ArgumentException("Expecting an array with at least 2 strings", nameof(items));
        _items = items;
        Items = items;
        DefaultNormalizedValue = 0.0;
        NormalizedValue = 0.0;
    }

    public AudioStringListParameter(string title, string[] items, string? units = null, int id = 0, string? shortTitle = null, int selectedItem = 0, AudioParameterFlags flags = AudioParameterFlags.CanAutomate | AudioParameterFlags.IsList) : base(title, units, id, shortTitle, 0, 0.0, flags)
    {
        if (items.Length < 2) throw new ArgumentException("Expecting an array with at least 2 strings", nameof(items));
        _items = items;
        Items = items;
        DefaultNormalizedValue = ToNormalized(selectedItem);
        SelectedItem = selectedItem;
    }
    
    public string[] Items
    {
        get => _items;
        private init
        {
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
            if ((uint)value >= (uint)Items.Length) throw new ArgumentOutOfRangeException(nameof(value), $"Invalid selected item {value}. Must the < {Items.Length}");
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
        if (StepCount < 1) return 0.0;

        return (int)(Math.Min(StepCount, normalizedValue * (StepCount + 1)));
    }

    public sealed override double ToNormalized(double plainValue)
    {
        if (StepCount < 1) return 0.0;

        return plainValue / StepCount;
    }
}