// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct IInterAppAudioPresetManager
    {
        private static partial ComResult runLoadPresetBrowser_ccw(IInterAppAudioPresetManager* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult runSavePresetBrowser_ccw(IInterAppAudioPresetManager* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult loadNextPreset_ccw(IInterAppAudioPresetManager* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult loadPreviousPreset_ccw(IInterAppAudioPresetManager* self)
        {
            throw new NotImplementedException();
        }
    }
}
