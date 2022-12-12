// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Basic Program List Description.
/// </summary>
/// <param name="Id">program list identifier</param>
/// <param name="Name">name of program list</param>
/// <param name="ProgramCount">number of programs in this list</param>
/// <seealso cref="IAudioControllerUnitInfo"/>
public sealed record AudioProgramListInfo(AudioProgramListId Id, string Name, int ProgramCount);