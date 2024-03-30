// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IInterAppAudioHost
    {
        private static partial ComResult getScreenSize_ToManaged(IInterAppAudioHost* self, LibVst.ViewRect* size, float* scale)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult connectedToHost_ToManaged(IInterAppAudioHost* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult switchToHost_ToManaged(IInterAppAudioHost* self)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult sendRemoteControlEvent_ToManaged(IInterAppAudioHost* self, uint @event)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult getHostIcon_ToManaged(IInterAppAudioHost* self, void** icon)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult scheduleEventFromUI_ToManaged(IInterAppAudioHost* self, LibVst.Event* @event)
        {
            throw new NotImplementedException();
        }
        
        private static partial LibVst.IInterAppAudioPresetManager* createPresetManager_ToManaged(IInterAppAudioHost* self, Guid* cid)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult showSettingsView_ToManaged(IInterAppAudioHost* self)
        {
            throw new NotImplementedException();
        }
    }
}
