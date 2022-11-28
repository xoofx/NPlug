// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    public partial struct IAudioProcessor
    {

#if DEBUG
        static IAudioProcessor()
        {
            Debug.Assert(AudioSampleSize.Float32 == (AudioSampleSize)SymbolicSampleSizes.kSample32);
            Debug.Assert(AudioSampleSize.Float64 == (AudioSampleSize)SymbolicSampleSizes.kSample64);
            Debug.Assert(NPlug.AudioBusDirection.Input == (NPlug.AudioBusDirection)BusDirections.kInput);
            Debug.Assert(NPlug.AudioBusDirection.Output == (NPlug.AudioBusDirection)BusDirections.kOutput);
            // TODO: Check SpeakerArrangement
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NPlug.IAudioProcessor Get(ComObject* self) => ((NPlug.IAudioProcessor)self->Handle.Target!);

        private static partial ComResult setBusArrangements_ccw(ComObject* self, SpeakerArrangement* inputs, int numIns, SpeakerArrangement* outputs, int numOuts)
        {
            if (numIns < 0 || numOuts < 0) return ComResult.InvalidArg;
            try
            {
                return Get(self).SetBusArrangements(new Span<NPlug.SpeakerArrangement>((NPlug.SpeakerArrangement*)inputs, numIns), new Span<NPlug.SpeakerArrangement>((NPlug.SpeakerArrangement*)outputs, numOuts));
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult getBusArrangement_ccw(ComObject* self, BusDirection dir, int index, SpeakerArrangement* arr)
        {
            if (index < 0) return ComResult.InvalidArg;
            try
            {
                *arr = (SpeakerArrangement)((ulong)Get(self).GetBusArrangement((NPlug.AudioBusDirection)dir.Value, index));
                return ComResult.Ok;
            }
            catch (ArgumentException)
            {
                return ComResult.InvalidArg;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult canProcessSampleSize_ccw(ComObject* self, int symbolicSampleSize)
        {
            try
            {
                return Get(self).CanProcessSampleSize((AudioSampleSize)symbolicSampleSize);
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial uint getLatencySamples_ccw(ComObject* self)
        {
            try
            {
                return Get(self).LatencySamples;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static partial ComResult setupProcessing_ccw(ComObject* self, ProcessSetup* setup)
        {
            try
            {
                return Get(self).SetupProcessing(*(AudioProcessSetupData*)setup);
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult setProcessing_ccw(ComObject* self, bool state)
        {
            try
            {
                Get(self).SetProcessing(state);
                return ComResult.Ok;
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial ComResult process_ccw(ComObject* self, ProcessData* data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
                return ComResult.InternalError;
            }
        }

        private static partial uint getTailSamples_ccw(ComObject* self)
        {
            return Get(self).TailSamples;
        }
    }
}