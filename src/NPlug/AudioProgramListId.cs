// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public readonly record struct AudioProgramListId(int Value)
{
    public static readonly AudioProgramListId NoPrograms = new (-1);

    public static implicit operator AudioProgramListId(int value) => new(value);
}