// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Backend;

/// <summary>
/// Backend interface to interact with <see cref="AudioAttributeList"/>.
/// </summary>
public interface IAudioAttributeListBackend
{
    /// <summary>
    /// Tries to set a value for the specified attribute.
    /// </summary>
    /// <param name="attributeList">The associated attribute list.</param>
    /// <param name="attributeId">The name of the attribute.</param>
    /// <param name="value">The value to set.</param>
    /// <returns><c>true</c> if the value was successfully set; <c>false</c> otherwise</returns>
    bool TrySetInt64(in AudioAttributeList attributeList, string attributeId, long value);
    /// <summary>
    /// Tries to get a value for the specified attribute.
    /// </summary>
    /// <param name="attributeList">The associated attribute list.</param>
    /// <param name="attributeId">The name of the attribute.</param>
    /// <param name="value">The value to get.</param>
    /// <returns><c>true</c> if the value was successfully get; <c>false</c> otherwise</returns>
    bool TryGetInt64(in AudioAttributeList attributeList, string attributeId, out long value);
    /// <summary>
    /// Tries to set a value for the specified attribute.
    /// </summary>
    /// <param name="attributeList">The associated attribute list.</param>
    /// <param name="attributeId">The name of the attribute.</param>
    /// <param name="value">The value to set.</param>
    /// <returns><c>true</c> if the value was successfully set; <c>false</c> otherwise</returns>
    bool TrySetFloat64(in AudioAttributeList attributeList, string attributeId, double value);
    /// <summary>
    /// Tries to get a value for the specified attribute.
    /// </summary>
    /// <param name="attributeList">The associated attribute list.</param>
    /// <param name="attributeId">The name of the attribute.</param>
    /// <param name="value">The value to get.</param>
    /// <returns><c>true</c> if the value was successfully get; <c>false</c> otherwise</returns>
    bool TryGetFloat64(in AudioAttributeList attributeList, string attributeId, out double value);
    /// <summary>
    /// Tries to set a value for the specified attribute.
    /// </summary>
    /// <param name="attributeList">The associated attribute list.</param>
    /// <param name="attributeId">The name of the attribute.</param>
    /// <param name="value">The value to set.</param>
    /// <returns><c>true</c> if the value was successfully set; <c>false</c> otherwise</returns>
    bool TrySetString(in AudioAttributeList attributeList, string attributeId, string value);
    /// <summary>
    /// Tries to get a value for the specified attribute.
    /// </summary>
    /// <param name="attributeList">The associated attribute list.</param>
    /// <param name="attributeId">The name of the attribute.</param>
    /// <param name="value">The value to get.</param>
    /// <returns><c>true</c> if the value was successfully get; <c>false</c> otherwise</returns>
    bool TryGetString(in AudioAttributeList attributeList, string attributeId, out string value);
    /// <summary>
    /// Tries to set a value for the specified attribute.
    /// </summary>
    /// <param name="attributeList">The associated attribute list.</param>
    /// <param name="attributeId">The name of the attribute.</param>
    /// <param name="value">The value to set.</param>
    /// <returns><c>true</c> if the value was successfully set; <c>false</c> otherwise</returns>
    bool TrySetBinary(in AudioAttributeList attributeList, string attributeId, ReadOnlySpan<byte> value);

    /// <summary>
    /// Tries to get a value for the specified attribute.
    /// </summary>
    /// <param name="attributeList">The associated attribute list.</param>
    /// <param name="attributeId">The name of the attribute.</param>
    /// <param name="value">The value to get.</param>
    /// <returns><c>true</c> if the value was successfully get; <c>false</c> otherwise</returns>
    bool TryGetBinary(in AudioAttributeList attributeList, string attributeId, out ReadOnlySpan<byte> value);
}