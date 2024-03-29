// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics;
using System.Drawing;

namespace NPlug;

/// <summary>
/// A rectangle with left, top, right, bottom coordinates on screen.
/// </summary>
/// <param name="Left"></param>
/// <param name="Top"></param>
/// <param name="Right"></param>
/// <param name="Bottom"></param>
public readonly record struct ViewRectangle(int Left, int Top, int Right, int Bottom)
{
    /// <summary>
    /// Gets the location of this rectangle.
    /// </summary>
    public Point Location => new(Left, Top);

    /// <summary>
    /// Gets the size of this rectangle.
    /// </summary>
    public Size Size => new(Right - Left, Bottom - Top);
}