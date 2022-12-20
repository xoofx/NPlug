// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.IO;

namespace NPlug;

public abstract partial class AudioProcessor<TAudioProcessorModel>
    : IAudioProcessorProgramListData
{
    bool IAudioProcessorProgramListData.IsProgramDataSupported(AudioProgramListId listId)
    {
        return Model.ContainsProgramList(listId);
    }

    void IAudioProcessorProgramListData.GetProgramData(AudioProgramListId listId, int programIndex, Stream output)
    {
        var programDataStream = Model.GetProgramListById(listId)[programIndex].GetProgramData();
        programDataStream?.CopyTo(output);
    }

    void IAudioProcessorProgramListData.SetProgramData(AudioProgramListId listId, int programIndex, Stream input)
    {
        var program = Model.GetProgramListById(listId)[programIndex];
        program.SetProgramDataFromStream(input);
    }
}