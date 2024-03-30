// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.
namespace NPlug.Interop;


using System;

internal static unsafe partial class LibVst
{
    public partial struct IVst3WrapperMPESupport
    {
        private static partial ComResult enableMPEInputProcessing_ToManaged(IVst3WrapperMPESupport* self, byte state)
        {
            throw new NotImplementedException();
        }
        
        private static partial ComResult setMPEInputDeviceSettings_ToManaged(IVst3WrapperMPESupport* self, int masterChannel, int memberBeginChannel, int memberEndChannel)
        {
            throw new NotImplementedException();
        }
    }
}
