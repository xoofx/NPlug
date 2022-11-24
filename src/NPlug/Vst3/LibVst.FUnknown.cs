// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace NPlug.Vst3;

internal static unsafe partial class LibVst
{
    //public static void** InitializeVtbl_FUnknown(void** vtbl)
    //{
    //    *vtbl++ = (delegate* unmanaged<ComObject*, Guid*, void**, int>)&FUnknown_queryInterface;
    //    *vtbl++ = (delegate* unmanaged<ComObject*, uint>)&FUnknown_addRef;
    //    *vtbl++ = (delegate* unmanaged<ComObject*, uint>)&FUnknown_release;
    //    return vtbl;
    //}

    public partial struct FUnknown
    {
        private static partial ComResult queryInterface_ccw(ComObject* pObj, Guid _iid, void** pInterface)
        {
            *pInterface = (void*)0;
            // TODO implement
            //if (pObj->Handle.Target is ObjectUnknown objUnknown && objUnknown.QueryInterface(_iid, out var obj))
            //{
            //    *pInterface = (void*)obj.Pointer;
            //    return ComResult.Ok;
            //}

            return ComResult.NoInterface;
        }

        private static partial uint addRef_ccw(ComObject* pObj)
        {
            bool lockTaken = false;
            try
            {
                pObj->Lock.Enter(ref lockTaken);
                pObj->RefCount++;
                return pObj->RefCount;
            }
            finally
            {
                pObj->Lock.Exit();
            }
        }

        private static partial uint release_ccw(ComObject* pObj)
        {
            bool lockTaken = false;
            try
            {
                pObj->Lock.Enter(ref lockTaken);
                if (pObj->RefCount > 0)
                {
                    pObj->RefCount--;
                    if (pObj->RefCount == 0 && pObj->Handle.IsAllocated)
                    {
                        pObj->Handle.Free();
                        pObj->Handle = default;
                    }
                }

                return pObj->RefCount;
            }
            finally
            {
                pObj->Lock.Exit();
            }
        }
    }
}