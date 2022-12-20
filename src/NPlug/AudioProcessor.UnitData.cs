// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.IO;
using NPlug.IO;

namespace NPlug;

public abstract partial class AudioProcessor<TAudioProcessorModel>
    : IAudioProcessorUnitData
{
    bool IAudioProcessorUnitData.IsUnitDataSupported(AudioUnitId unitId)
    {
        return Model.TryGetUnitById(unitId, out _);
    }

    void IAudioProcessorUnitData.GetUnitData(AudioUnitId unitId, Stream output)
    {
        if (Model.TryGetUnitById(unitId, out var unit))
        {
            unit.Save(new PortableBinaryWriter(output, false), AudioProcessorModelStorageMode.Default);
        }
    }

    void IAudioProcessorUnitData.SetUnitData(AudioUnitId unitId, Stream input)
    {
        if (Model.TryGetUnitById(unitId, out var unit))
        {
            unit.Load(new PortableBinaryReader(input, false), AudioProcessorModelStorageMode.Default);
        }
    }
}