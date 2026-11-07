// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug;

/// <summary>
/// The note-on articulations of the MIDI 2.0 orchestral profile that a plugin supports on a channel: for each
/// <see cref="AudioOrchestralArticulationClassification"/> and each of its 16 subclasses, the number of variations
/// (0 = the subclass is not available).
/// </summary>
/// <remarks>
/// This is the equivalent of <c>Vst::NoteOnOrchestralArticulation::ClassificationVariations</c> (VST 3.8.1), with the
/// same memory layout (17 × 16 bytes). A classification whose 16 entries are all 0 is not available.
/// </remarks>
public struct AudioOrchestralArticulationVariations
{
    /// <summary>
    /// Number of classifications (<see cref="AudioOrchestralArticulationClassification"/>).
    /// </summary>
    public const int ClassificationCount = 17;

    /// <summary>
    /// Number of subclasses of a classification.
    /// </summary>
    public const int SubClassCount = 16;

    private Buffer _variations;

    /// <summary>
    /// Gets or sets the number of variations of a subclass (0 = not available).
    /// </summary>
    /// <param name="classification">The classification.</param>
    /// <param name="subClass">The subclass, 0 to 15, as indexed in the MIDI 2.0 orchestral profile.</param>
    /// <exception cref="ArgumentOutOfRangeException">The classification or the subclass is out of range.</exception>
    public byte this[AudioOrchestralArticulationClassification classification, int subClass]
    {
        readonly get => _variations[IndexOf(classification, subClass)];
        set => _variations[IndexOf(classification, subClass)] = value;
    }

    /// <summary>
    /// Returns <c>true</c> if at least one subclass of the classification has a variation.
    /// </summary>
    /// <param name="classification">The classification.</param>
    /// <returns><c>true</c> if the classification is available.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The classification is out of range.</exception>
    public readonly bool IsAvailable(AudioOrchestralArticulationClassification classification)
    {
        int start = IndexOf(classification, 0);
        ReadOnlySpan<byte> all = _variations;
        return all.Slice(start, SubClassCount).ContainsAnyExcept((byte)0);
    }

    /// <summary>
    /// Sets every entry to 0 (nothing available).
    /// </summary>
    public void Clear() => ((Span<byte>)_variations).Clear();

    private static int IndexOf(AudioOrchestralArticulationClassification classification, int subClass)
    {
        if ((uint)classification >= ClassificationCount) throw new ArgumentOutOfRangeException(nameof(classification), classification, $"The classification must be below {ClassificationCount}.");
        if ((uint)subClass >= SubClassCount) throw new ArgumentOutOfRangeException(nameof(subClass), subClass, $"The subclass must be between 0 and {SubClassCount - 1}.");
        return (int)classification * SubClassCount + subClass;
    }

    [InlineArray(ClassificationCount * SubClassCount)]
    private struct Buffer
    {
        private byte _element0;
    }
}
