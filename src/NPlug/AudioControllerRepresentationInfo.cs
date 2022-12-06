// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// RepresentationInfo is the structure describing a representation
/// This structure is used in the function
/// </summary>
/// <param name="Vendor">Vendor name of the associated representation (remote) (eg. "Yamaha").</param>
/// <param name="Name">Representation (remote) Name (eg. "O2").</param>
/// <param name="Version">Version of this "Remote" (eg. "1.0").</param>
/// <param name="Host">Optional: used if the representation is for a given host only (eg. "Nuendo").</param>
/// <seealso cref="IAudioControllerXmlRepresentation "/>
/// <seealso cref="IAudioControllerXmlRepresentation.GetXmlRepresentationStream."/>
public readonly record struct AudioControllerRepresentationInfo(string Vendor, string Name, string Version, string Host);