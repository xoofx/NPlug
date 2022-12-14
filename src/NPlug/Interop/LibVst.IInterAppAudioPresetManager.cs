// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IInterAppAudioPresetManager
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioControllerInterAppAudioPresetManager Get(IInterAppAudioPresetManager* self) => ((ComObjectHandle*)self)->As<IAudioControllerInterAppAudioPresetManager>();

        private static partial ComResult runLoadPresetBrowser_ToManaged(IInterAppAudioPresetManager* self)
        {
            Get(self).RunLoadPresetBrowser();
            return true;
        }

        private static partial ComResult runSavePresetBrowser_ToManaged(IInterAppAudioPresetManager* self)
        {
            Get(self).RunSavePresetBrowser();
            return true;
        }

        private static partial ComResult loadNextPreset_ToManaged(IInterAppAudioPresetManager* self)
        {
            Get(self).LoadNextPreset();
            return true;
        }

        private static partial ComResult loadPreviousPreset_ToManaged(IInterAppAudioPresetManager* self)
        {
            Get(self).LoadPreviousPreset();
            return true;
        }
    }
}
