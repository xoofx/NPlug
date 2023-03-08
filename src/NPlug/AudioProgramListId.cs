// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// An id associated with a <see cref="AudioProgramList"/>.
/// </summary>
/// <param name="Value"></param>
public readonly record struct AudioProgramListId(int Value)
{
    /// <summary>
    /// Default value for <see cref="AudioProgramListId"/> when there is no program list.
    /// </summary>
    public static readonly AudioProgramListId NoPrograms = new (-1);

    /// <summary>
    /// Converts an <see cref="int"/> to an <see cref="AudioProgramListId"/>.
    /// </summary>
    public static implicit operator AudioProgramListId(int value) => new(value);
}