// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// A parameter id associated with a parameter <see cref="AudioParameter.Id"/>.
/// </summary>
/// <param name="Value">The id of the parameter.</param>
public readonly record struct AudioParameterId(int Value)
{
    /// <summary>
    /// Converts an integer to a <see cref="AudioParameterId"/>.
    /// </summary>
    public static implicit operator AudioParameterId(int value) => new(value);
}