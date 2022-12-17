// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.IO;

namespace NPlug;

public abstract partial class AudioProcessor<TAudioProcessorModel>
    : IAudioProcessorProgramListData
{
    public bool IsProgramDataSupported(AudioProgramListId listId)
    {
        return Model.ContainsProgramList(listId);
    }

    public void GetProgramData(AudioProgramListId listId, int programIndex, Stream output)
    {
        var programDataStream = Model.GetProgramListById(listId)[programIndex].GetOrLoadProgramData();
        programDataStream.CopyTo(output);
    }

    public void SetProgramData(AudioProgramListId listId, int programIndex, Stream input)
    {
        var program = Model.GetProgramListById(listId)[programIndex];
        program.LoadProgramDataFromStream(input);
    }
}