// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Numerics;

namespace NPlug;

[Flags]
public enum SpeakerArrangement : ulong
{
    SpeakerL = 1 << 0,

    SpeakerR = 1 << 1,

    SpeakerC = 1 << 2,

    SpeakerLfe = 1 << 3,

    SpeakerLs = 1 << 4,

    SpeakerRs = 1 << 5,

    SpeakerLc = 1 << 6,

    SpeakerRc = 1 << 7,

    SpeakerS = 1 << 8,

    SpeakerCs = SpeakerS,

    SpeakerSl = 1 << 9,

    SpeakerSr = 1 << 10,

    SpeakerTc = 1 << 11,

    SpeakerTfl = 1 << 12,

    SpeakerTfc = 1 << 13,

    SpeakerTfr = 1 << 14,

    SpeakerTrl = 1 << 15,

    SpeakerTrc = 1 << 16,

    SpeakerTrr = 1 << 17,

    SpeakerLfe2 = 1 << 18,

    SpeakerM = 1 << 19,

    SpeakerACN0 = 1UL << 20,

    SpeakerACN1 = 1UL << 21,

    SpeakerACN2 = 1UL << 22,

    SpeakerACN3 = 1UL << 23,

    SpeakerACN4 = 1UL << 38,

    SpeakerACN5 = 1UL << 39,

    SpeakerACN6 = 1UL << 40,

    SpeakerACN7 = 1UL << 41,

    SpeakerACN8 = 1UL << 42,

    SpeakerACN9 = 1UL << 43,

    SpeakerACN10 = 1UL << 44,

    SpeakerACN11 = 1UL << 45,

    SpeakerACN12 = 1UL << 46,

    SpeakerACN13 = 1UL << 47,

    SpeakerACN14 = 1UL << 48,

    SpeakerACN15 = 1UL << 49,

    SpeakerTsl = 1UL << 24,

    SpeakerTsr = 1UL << 25,

    SpeakerLcs = 1UL << 26,

    SpeakerRcs = 1UL << 27,

    SpeakerBfl = 1UL << 28,

    SpeakerBfc = 1UL << 29,

    SpeakerBfr = 1UL << 30,

    SpeakerPl = 1UL << 31,

    SpeakerPr = 1UL << 32,

    SpeakerBsl = 1UL << 33,

    SpeakerBsr = 1UL << 34,

    SpeakerBrl = 1UL << 35,

    SpeakerBrc = 1UL << 36,

    SpeakerBrr = 1UL << 37,

    SpeakerEmpty = 0,

    SpeakerMono = SpeakerM,

    SpeakerStereo = SpeakerL | SpeakerR,

    SpeakerStereoSurround = SpeakerLs | SpeakerRs,

    SpeakerStereoCenter = SpeakerLc | SpeakerRc,

    SpeakerStereoSide = SpeakerSl | SpeakerSr,

    SpeakerStereoCLfe = SpeakerC | SpeakerLfe,

    SpeakerStereoTF = SpeakerTfl | SpeakerTfr,

    SpeakerStereoTS = SpeakerTsl | SpeakerTsr,

    SpeakerStereoTR = SpeakerTrl | SpeakerTrr,

    SpeakerStereoBF = SpeakerBfl | SpeakerBfr,

    SpeakerCineFront = SpeakerL | SpeakerR | SpeakerC | SpeakerLc | SpeakerRc,

    Speaker30Cine = SpeakerL | SpeakerR | SpeakerC,

    Speaker31Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe,

    Speaker30Music = SpeakerL | SpeakerR | SpeakerCs,

    Speaker31Music = SpeakerL | SpeakerR | SpeakerLfe | SpeakerCs,

    Speaker40Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerCs,

    Speaker41Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerCs,

    Speaker40Music = SpeakerL | SpeakerR | SpeakerLs | SpeakerRs,

    Speaker41Music = SpeakerL | SpeakerR | SpeakerLfe | SpeakerLs | SpeakerRs,

    Speaker50 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs,

    Speaker51 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs,

    Speaker60Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerCs,

    Speaker61Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerCs,

    Speaker60Music = SpeakerL | SpeakerR | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr,

    Speaker61Music = SpeakerL | SpeakerR | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr,

    Speaker70Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc,

    Speaker71Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc,

    Speaker71CineFullFront = Speaker71Cine,

    Speaker70Music = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr,

    Speaker71Music = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr,

    Speaker71CineFullRear = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerLcs | SpeakerRcs,

    Speaker71CineSideFill = Speaker71Music,

    Speaker71Proximity = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerPl | SpeakerPr,

    Speaker80Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerCs,

    Speaker81Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerCs,

    Speaker80Music = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerCs | SpeakerSl | SpeakerSr,

    Speaker81Music = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerCs | SpeakerSl | SpeakerSr,

    Speaker90Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerSl | SpeakerSr,

    Speaker91Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerSl | SpeakerSr,

    Speaker100Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerCs | SpeakerSl | SpeakerSr,

    Speaker101Cine = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerCs | SpeakerSl | SpeakerSr,

    SpeakerAmbi1stOrderACN = SpeakerACN0 | SpeakerACN1 | SpeakerACN2 | SpeakerACN3,

    SpeakerAmbi2cdOrderACN = SpeakerAmbi1stOrderACN | SpeakerACN4 | SpeakerACN5 | SpeakerACN6 | SpeakerACN7 | SpeakerACN8,

    SpeakerAmbi3rdOrderACN = SpeakerAmbi2cdOrderACN | SpeakerACN9 | SpeakerACN10 | SpeakerACN11 | SpeakerACN12 | SpeakerACN13 | SpeakerACN14 | SpeakerACN15,

    Speaker80Cube = SpeakerL | SpeakerR | SpeakerLs | SpeakerRs | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker40_4 = Speaker80Cube,

    Speaker71CineTopCenter = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerCs | SpeakerTc,

    Speaker71CineCenterHigh = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerCs | SpeakerTfc,

    Speaker70CineFrontHigh = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerTfl | SpeakerTfr,

    Speaker70MPEG3D = Speaker70CineFrontHigh,

    Speaker50_2 = Speaker70CineFrontHigh,

    Speaker71CineFrontHigh = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerTfl | SpeakerTfr,

    Speaker71MPEG3D = Speaker71CineFrontHigh,

    Speaker51_2 = Speaker71CineFrontHigh,

    Speaker71CineSideHigh = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerTsl | SpeakerTsr,

    Speaker81MPEG3D = SpeakerL | SpeakerR | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerTfl | SpeakerTfc | SpeakerTfr | SpeakerBfc,

    Speaker41_4_1 = Speaker81MPEG3D,

    Speaker90 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker50_4 = Speaker90,

    Speaker91 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker51_4 = Speaker91,

    Speaker50_4_1 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr | SpeakerBfc,

    Speaker51_4_1 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr | SpeakerBfc,

    Speaker70_2 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr | SpeakerTsl | SpeakerTsr,

    Speaker71_2 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr | SpeakerTsl | SpeakerTsr,

    Speaker91Atmos = Speaker71_2,

    Speaker70_3 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr | SpeakerTfl | SpeakerTfr | SpeakerTrc,

    Speaker72_3 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr | SpeakerTfl | SpeakerTfr | SpeakerTrc | SpeakerLfe2,

    Speaker70_4 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker71_4 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker111MPEG3D = Speaker71_4,

    Speaker70_6 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr | SpeakerTsl | SpeakerTsr,

    Speaker71_6 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr | SpeakerTsl | SpeakerTsr,

    Speaker90_4 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerSl | SpeakerSr | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker91_4 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerSl | SpeakerSr | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker90_6 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerSl | SpeakerSr | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr | SpeakerTsl | SpeakerTsr,

    Speaker91_6 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerSl | SpeakerSr | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr | SpeakerTsl | SpeakerTsr,

    Speaker100 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerTc | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker50_5 = Speaker100,

    Speaker101 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerTc | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker101MPEG3D = Speaker101,

    Speaker51_5 = Speaker101,

    Speaker102 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerTfl | SpeakerTfc | SpeakerTfr | SpeakerTrl | SpeakerTrr | SpeakerLfe2,

    Speaker52_5 = Speaker102,

    Speaker110 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerTc | SpeakerTfl | SpeakerTfc | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker50_6 = Speaker110,

    Speaker111 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerTc | SpeakerTfl | SpeakerTfc | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker51_6 = Speaker111,

    Speaker122 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerTfl | SpeakerTfc | SpeakerTfr | SpeakerTrl | SpeakerTrr | SpeakerLfe2,

    Speaker72_5 = Speaker122,

    Speaker130 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr | SpeakerTc | SpeakerTfl | SpeakerTfc | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker131 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr | SpeakerTc | SpeakerTfl | SpeakerTfc | SpeakerTfr | SpeakerTrl | SpeakerTrr,

    Speaker140 = SpeakerL | SpeakerR | SpeakerLs | SpeakerRs | SpeakerSl | SpeakerSr | SpeakerTfl | SpeakerTfr | SpeakerTrl | SpeakerTrr | SpeakerBfl | SpeakerBfr | SpeakerBrl | SpeakerBrr,

    Speaker60_4_4 = Speaker140,

    Speaker220 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerCs | SpeakerSl | SpeakerSr | SpeakerTc | SpeakerTfl | SpeakerTfc | SpeakerTfr | SpeakerTrl | SpeakerTrc | SpeakerTrr | SpeakerTsl | SpeakerTsr | SpeakerBfl | SpeakerBfc | SpeakerBfr,

    Speaker100_9_3 = Speaker220,

    Speaker222 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerLc | SpeakerRc | SpeakerCs | SpeakerSl | SpeakerSr | SpeakerTc | SpeakerTfl | SpeakerTfc | SpeakerTfr | SpeakerTrl | SpeakerTrc | SpeakerTrr | SpeakerLfe2 | SpeakerTsl | SpeakerTsr | SpeakerBfl | SpeakerBfc | SpeakerBfr,

    Speaker102_9_3 = Speaker222,

    Speaker50_5_3 = SpeakerL | SpeakerR | SpeakerC | SpeakerLs | SpeakerRs | SpeakerTfl | SpeakerTfc | SpeakerTfr | SpeakerTrl | SpeakerTrr | SpeakerBfl | SpeakerBfc | SpeakerBfr,

    Speaker51_5_3 = SpeakerL | SpeakerR | SpeakerC | SpeakerLfe | SpeakerLs | SpeakerRs | SpeakerTfl | SpeakerTfc | SpeakerTfr | SpeakerTrl | SpeakerTrr | SpeakerBfl | SpeakerBfc | SpeakerBfr,
}

public static class SpeakerArrangementExtension
{
    public static int GetChannelCount(this SpeakerArrangement speakerArrangement) => BitOperations.PopCount((ulong)speakerArrangement);
}