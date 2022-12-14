// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct ITestPlugProvider2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioTestProvider Get(ITestPlugProvider2* self) => ((ComObjectHandle*)self)->As<IAudioTestProvider>();

        private static partial LibVst.IPluginFactory* getPluginFactory_ToManaged(ITestPlugProvider2* self)
        {
            return ComObjectManager.Instance.GetOrCreateComObject(Get(self).GetAudioProcessor()).QueryInterface<IPluginFactory>();
        }
    }
}
