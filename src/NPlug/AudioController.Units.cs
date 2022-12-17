// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.IO;

namespace NPlug;

public abstract partial class AudioController<TAudioRootUnit>
{
    private AudioUnit _selectedUnit;

    public AudioUnit SelectedUnit
    {
        get { return _selectedUnit; }
        set
        {
            if (value != _selectedUnit)
            {
                _selectedUnit = value;
                var handler = Handler;
                if (handler is not null && handler.IsUnitAndProgramListSupported)
                {
                    handler.NotifyUnitSelection(value.Id);
                }
            }
        }
    }

    int IAudioControllerUnitInfo.UnitCount => RootUnit.UnitCount;

    AudioUnitInfo IAudioControllerUnitInfo.GetUnitInfo(int unitIndex) => RootUnit.GetUnitByIndex(unitIndex).UnitInfo;

    AudioUnitId IAudioControllerUnitInfo.SelectedUnit
    {
        get => _selectedUnit.UnitInfo.Id;
        set
        {
            AudioUnit? nextUnit;
            if (value.Value == 0)
            {
                nextUnit = RootUnit;
            }
            else if (!RootUnit.TryGetUnitById(value, out nextUnit))
            {
                throw new ArgumentException($"Invalid Unit selected with id {value}. Unit does not exist!");
            }

            if (!ReferenceEquals(nextUnit, _selectedUnit))
            {
                _selectedUnit = nextUnit;
            }
        }
    }

    void IAudioControllerUnitInfo.GetUnitByBus(BusMediaType type, BusDirection dir, int busIndex, int channel, out AudioUnitId unitId)
    {
        throw new NotImplementedException();
    }

    void IAudioControllerUnitInfo.SetUnitProgramData(int listOrUnitId, int programIndex, Stream input)
    {
        throw new NotImplementedException();
    }
}