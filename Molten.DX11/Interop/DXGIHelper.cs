﻿using Silk.NET.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public unsafe delegate int DXGIEnumFunc<T>(uint index, ref T* ptrOutput) where T : unmanaged;

    internal static class DXGIHelper
    {
        public static HResult HResultFromDXGI(int dxgiResult)
        {
            const int FACDXGI = 0x87a;
            return HResult.Create(1, FACDXGI, dxgiResult);
        }

        public unsafe static DxgiError ErrorFromResult(int result)
        {
            return *(DxgiError*)&result;
        }

        /// <summary>
        /// Enumerates a DXGI list/function and returns a managed array 
        /// of pointers to enumerated DXGI objects of the given type.
        /// </summary>
        /// <typeparam name="T">The type of object to enumerate.</typeparam>
        /// <param name="enumFunc">The DXGI enum function to invoke when iterating.</param>
        /// <returns></returns>
        public unsafe static T*[] EnumArray<T>(DXGIEnumFunc<T> enumFunc)
            where T : unmanaged
        {
            uint count = GetEnumCount<T>(enumFunc);

            // Now build the array
            T*[] result = new T*[count];
            for (uint i = 0; i < count; i++)
            {
                enumFunc(i, ref result[i]);
            }

            return result;
        }

        /// <summary>
        /// Enumerates a DXGI list/function and returns a managed array 
        /// of pointers to enumerated DXGI objects of the given type.
        /// </summary>
        /// <typeparam name="T">The type of object to enumerate.</typeparam>
        /// <typeparam name="C">The intermediate type to be converted to <typeparamref name="T"/></typeparam>
        /// <param name="enumFunc">The DXGI enum function to invoke when iterating.</param>
        /// <returns></returns>
        public unsafe static T*[] EnumArray<T, C>(DXGIEnumFunc<C> enumFunc)
            where T : unmanaged
            where C: unmanaged
        {
            uint count = GetEnumCount<C>(enumFunc);

            // Now build the array
            T*[] result = new T*[count];
            for (uint i = 0; i < count; i++)
            {
                C* tVal = null;
                enumFunc(i, ref tVal);
                result[i] = (T*)tVal;
            }

            return result;
        }

        /// <summary>
        /// Returns the number of enumerable items available from a DXGI function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumFunc"></param>
        /// <returns></returns>
        public unsafe static uint GetEnumCount<T>(DXGIEnumFunc<T> enumFunc)
            where T : unmanaged
        {
            DxgiError err = DxgiError.Ok;
            T* temp = null;
            uint count = 0;

            // Find out how many items there are.
            while (err != DxgiError.Ok)
            {
                int r = enumFunc(count, ref temp);
                err = ErrorFromResult(r);

                if (err != DxgiError.Ok)
                    count++;
            }

            return count;
        }
    }
}
