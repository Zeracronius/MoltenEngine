using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Molten
{
    public static class StringHelper
    {
        public static byte[] GetBytes(object o)
        {
            int size = Marshal.SizeOf(o);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(o, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static byte[] GetBytes(string str, Encoding encoding)
        {
            return encoding.GetBytes(str); ;
        }

        public static string ConcatArray(string[] array, int startIndex, int count)
        {
            string result = "";
            int endIndex = startIndex + count;

            if (startIndex < 0 || startIndex >= array.Length)
                throw new Exception("The start index is out of range. Must be greater or equal to 0 and less than array length.");

            if (endIndex >= array.Length)
                throw new IndexOutOfRangeException("The startIndex and count would go beyond the string array capacity.");

            for (int i = startIndex; i < endIndex; i++)
                result += array[i];

            return result;
        }
    }
}
