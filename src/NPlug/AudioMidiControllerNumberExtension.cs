// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Globalization;

namespace NPlug;

/// <summary>
/// Extension methods for <see cref="AudioMidiControllerNumber"/>.
/// </summary>
public static class AudioMidiControllerNumberExtension
{
    /// <summary>
    /// Return a string representation of a controller number
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToText(this AudioMidiControllerNumber value)
    {
        return value switch
        {
            AudioMidiControllerNumber.BankSelectMSB => nameof(AudioMidiControllerNumber.BankSelectMSB),
            AudioMidiControllerNumber.ModWheel => nameof(AudioMidiControllerNumber.ModWheel),
            AudioMidiControllerNumber.Breath => nameof(AudioMidiControllerNumber.Breath),
            AudioMidiControllerNumber.Foot => nameof(AudioMidiControllerNumber.Foot),
            AudioMidiControllerNumber.PortamentoTime => nameof(AudioMidiControllerNumber.PortamentoTime),
            AudioMidiControllerNumber.DataEntryMSB => nameof(AudioMidiControllerNumber.DataEntryMSB),
            AudioMidiControllerNumber.Volume => nameof(AudioMidiControllerNumber.Volume),
            AudioMidiControllerNumber.Balance => nameof(AudioMidiControllerNumber.Balance),
            AudioMidiControllerNumber.Pan => nameof(AudioMidiControllerNumber.Pan),
            AudioMidiControllerNumber.Expression => nameof(AudioMidiControllerNumber.Expression),
            AudioMidiControllerNumber.Effect1 => nameof(AudioMidiControllerNumber.Effect1),
            AudioMidiControllerNumber.Effect2 => nameof(AudioMidiControllerNumber.Effect2),
            AudioMidiControllerNumber.GeneralPurposeController1 => nameof(AudioMidiControllerNumber.GeneralPurposeController1),
            AudioMidiControllerNumber.GeneralPurposeController2 => nameof(AudioMidiControllerNumber.GeneralPurposeController2),
            AudioMidiControllerNumber.GeneralPurposeController3 => nameof(AudioMidiControllerNumber.GeneralPurposeController3),
            AudioMidiControllerNumber.GeneralPurposeController4 => nameof(AudioMidiControllerNumber.GeneralPurposeController4),
            AudioMidiControllerNumber.BankSelectLSB => nameof(AudioMidiControllerNumber.BankSelectLSB),
            AudioMidiControllerNumber.DataEntryLSB => nameof(AudioMidiControllerNumber.DataEntryLSB),
            AudioMidiControllerNumber.SustainOnOff => nameof(AudioMidiControllerNumber.SustainOnOff),
            AudioMidiControllerNumber.PortamentoOnOff => nameof(AudioMidiControllerNumber.PortamentoOnOff),
            AudioMidiControllerNumber.SustenutoOnOff => nameof(AudioMidiControllerNumber.SustenutoOnOff),
            AudioMidiControllerNumber.SoftPedalOnOff => nameof(AudioMidiControllerNumber.SoftPedalOnOff),
            AudioMidiControllerNumber.LegatoFootSwitchOnOff => nameof(AudioMidiControllerNumber.LegatoFootSwitchOnOff),
            AudioMidiControllerNumber.Hold2OnOff => nameof(AudioMidiControllerNumber.Hold2OnOff),
            AudioMidiControllerNumber.SoundVariation => nameof(AudioMidiControllerNumber.SoundVariation),
            AudioMidiControllerNumber.FilterCutoff => nameof(AudioMidiControllerNumber.FilterCutoff),
            AudioMidiControllerNumber.ReleaseTime => nameof(AudioMidiControllerNumber.ReleaseTime),
            AudioMidiControllerNumber.AttackTime => nameof(AudioMidiControllerNumber.AttackTime),
            AudioMidiControllerNumber.FilterResonance => nameof(AudioMidiControllerNumber.FilterResonance),
            AudioMidiControllerNumber.DecayTime => nameof(AudioMidiControllerNumber.DecayTime),
            AudioMidiControllerNumber.VibratoRate => nameof(AudioMidiControllerNumber.VibratoRate),
            AudioMidiControllerNumber.VibratoDepth => nameof(AudioMidiControllerNumber.VibratoDepth),
            AudioMidiControllerNumber.VibratoDelay => nameof(AudioMidiControllerNumber.VibratoDelay),
            AudioMidiControllerNumber.SoundController10 => nameof(AudioMidiControllerNumber.SoundController10),
            AudioMidiControllerNumber.GeneralPurposeController5 => nameof(AudioMidiControllerNumber.GeneralPurposeController5),
            AudioMidiControllerNumber.GeneralPurposeController6 => nameof(AudioMidiControllerNumber.GeneralPurposeController6),
            AudioMidiControllerNumber.GeneralPurposeController7 => nameof(AudioMidiControllerNumber.GeneralPurposeController7),
            AudioMidiControllerNumber.GeneralPurposeController8 => nameof(AudioMidiControllerNumber.GeneralPurposeController8),
            AudioMidiControllerNumber.PortamentoControl => nameof(AudioMidiControllerNumber.PortamentoControl),
            AudioMidiControllerNumber.Effect1Depth => nameof(AudioMidiControllerNumber.Effect1Depth),
            AudioMidiControllerNumber.Effect2Depth => nameof(AudioMidiControllerNumber.Effect2Depth),
            AudioMidiControllerNumber.Effect3Depth => nameof(AudioMidiControllerNumber.Effect3Depth),
            AudioMidiControllerNumber.Effect4Depth => nameof(AudioMidiControllerNumber.Effect4Depth),
            AudioMidiControllerNumber.Effect5Depth => nameof(AudioMidiControllerNumber.Effect5Depth),
            AudioMidiControllerNumber.DataIncrement => nameof(AudioMidiControllerNumber.DataIncrement),
            AudioMidiControllerNumber.DataDecrement => nameof(AudioMidiControllerNumber.DataDecrement),
            AudioMidiControllerNumber.NRPNSelectLSB => nameof(AudioMidiControllerNumber.NRPNSelectLSB),
            AudioMidiControllerNumber.NRPNSelectMSB => nameof(AudioMidiControllerNumber.NRPNSelectMSB),
            AudioMidiControllerNumber.RPNSelectLSB => nameof(AudioMidiControllerNumber.RPNSelectLSB),
            AudioMidiControllerNumber.RPNSelectMSB => nameof(AudioMidiControllerNumber.RPNSelectMSB),
            AudioMidiControllerNumber.AllSoundsOff => nameof(AudioMidiControllerNumber.AllSoundsOff),
            AudioMidiControllerNumber.ResetAllControllers => nameof(AudioMidiControllerNumber.ResetAllControllers),
            AudioMidiControllerNumber.LocalCtrlOnOff => nameof(AudioMidiControllerNumber.LocalCtrlOnOff),
            AudioMidiControllerNumber.AllNotesOff => nameof(AudioMidiControllerNumber.AllNotesOff),
            AudioMidiControllerNumber.OmniModeOff => nameof(AudioMidiControllerNumber.OmniModeOff),
            AudioMidiControllerNumber.OmniModeOn => nameof(AudioMidiControllerNumber.OmniModeOn),
            AudioMidiControllerNumber.PolyModeOnOff => nameof(AudioMidiControllerNumber.PolyModeOnOff),
            AudioMidiControllerNumber.PolyModeOn => nameof(AudioMidiControllerNumber.PolyModeOn),
            AudioMidiControllerNumber.AfterTouch => nameof(AudioMidiControllerNumber.AfterTouch),
            AudioMidiControllerNumber.PitchBend => nameof(AudioMidiControllerNumber.PitchBend),
            AudioMidiControllerNumber.CountControllerNumber => nameof(AudioMidiControllerNumber.CountControllerNumber),
            AudioMidiControllerNumber.PolyPressure => nameof(AudioMidiControllerNumber.PolyPressure),
            AudioMidiControllerNumber.QuarterFrame => nameof(AudioMidiControllerNumber.QuarterFrame),
            _ => ((int)value).ToString(CultureInfo.InvariantCulture),
        };
    }
}