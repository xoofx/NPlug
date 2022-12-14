// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace NPlug.Interop;

internal static unsafe partial class LibVst
{
    public partial struct ITestPlugProvider
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IAudioTestProvider Get(ITestPlugProvider* self) => ((ComObjectHandle*)self)->As<IAudioTestProvider>();
        
        private static partial LibVst.IComponent* getComponent_ToManaged(ITestPlugProvider* self)
        {
            return ComObjectManager.Instance.GetOrCreateComObject(Get(self).GetAudioProcessor()).QueryInterface<IComponent>();
        }
        
        private static partial LibVst.IEditController* getController_ToManaged(ITestPlugProvider* self)
        {
            return ComObjectManager.Instance.GetOrCreateComObject(Get(self).GetAudioController()).QueryInterface<IEditController>();
        }

        private static partial ComResult releasePlugIn_ToManaged(ITestPlugProvider* self, LibVst.IComponent* component, LibVst.IEditController* controller)
        {
            // Releasing the native objects will release automatically the managed objects.
            if (component != null)
            {
                ((FUnknown*)component)->release();
            }

            if (controller != null)
            {
                ((FUnknown*)controller)->release();
            }

            return true;
        }
        
        private static partial ComResult getSubCategories_ToManaged(ITestPlugProvider* self, LibVst.IStringResult* result)
        {
            var subCategory = GetPluginSubCategory(Get(self).GetAudioProcessorCategory());
            var array = ArrayPool<byte>.Shared.Rent(129);
            try
            {
                var length = Encoding.UTF8.GetBytes(subCategory, array);
                array[length] = 0;
                fixed (byte* arrayPtr = array)
                {
                    result->setText(arrayPtr);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }

            return true;
        }
        
        private static partial ComResult getComponentUID_ToManaged(ITestPlugProvider* self, LibVst.FUID* uid)
        {
            *((Guid*)uid) = Get(self).GetAudioProcessorClassId();
            return true;
        }
    }
}
