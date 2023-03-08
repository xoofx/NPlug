// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Defines the progress id used when calling <see cref="IAudioControllerHandler.StartProgress"/>.
/// </summary>
/// <param name="Value">An internal representation of a progress.</param>
public record struct AudioProgressId(ulong Value);