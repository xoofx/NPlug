// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug;

[AttributeUsage(AttributeTargets.Method)]
public sealed class AudioPluginFactoryEntryPointAttribute : Attribute
{
}