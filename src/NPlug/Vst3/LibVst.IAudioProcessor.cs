// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IAudioProcessor
    {
        private static partial ComResult setBusArrangements_ccw(ComObject* self, SpeakerArrangement* inputs, int numIns, SpeakerArrangement* outputs, int numOuts)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult getBusArrangement_ccw(ComObject* self, BusDirection dir, int index, SpeakerArrangement* arr)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult canProcessSampleSize_ccw(ComObject* self, int symbolicSampleSize)
        {
            throw new NotImplementedException();
        }

        private static partial uint getLatencySamples_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setupProcessing_ccw(ComObject* self, ProcessSetup* setup)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult setProcessing_ccw(ComObject* self, bool state)
        {
            throw new NotImplementedException();
        }

        private static partial ComResult process_ccw(ComObject* self, ProcessData* data)
        {
            throw new NotImplementedException();
        }

        private static partial uint getTailSamples_ccw(ComObject* self)
        {
            throw new NotImplementedException();
        }
    }
}