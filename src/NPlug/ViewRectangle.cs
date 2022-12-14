// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// A rectangle with left, top, right, bottom coordinates on screen.
/// </summary>
/// <param name="Left"></param>
/// <param name="Top"></param>
/// <param name="Right"></param>
/// <param name="Bottom"></param>
public readonly record struct ViewRectangle(int Left, int Top, int Right, int Bottom);