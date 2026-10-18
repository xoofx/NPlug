// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

// Lets the test project drive the interop layer (IPlugView / IPlugFrame vtables) directly.
[assembly: InternalsVisibleTo("NPlug.Tests")]
