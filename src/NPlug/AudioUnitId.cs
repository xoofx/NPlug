// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// A unit id associated with a unit <see cref="AudioUnit.Id"/>.
/// </summary>
public readonly record struct AudioUnitId(int Value)
{
    /// <summary>
    /// Defines a unit with no parent.
    /// </summary>
    public static readonly AudioUnitId NoParent = new (-1);

    /// <summary>
    /// Converts an integer to a <see cref="AudioUnitId"/>.
    /// </summary>
    public static implicit operator AudioUnitId(int value) => new(value);
}