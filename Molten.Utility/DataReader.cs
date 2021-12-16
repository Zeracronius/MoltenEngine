using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten
{
    public unsafe class DataReader
    {
        byte[] _data;
        int _offset;

        public DataReader(byte[] data)
        {
            _data = data;
            _offset = 0;
        }


        public T Read<T>() where T : unmanaged
        {
            T output;
            Read(&output, 1);
            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="output">Destination of read bytes.</param>
        /// <param name="elements">Number of elements to read.</param>
        /// <returns></returns>
        public void Read<T>(T* output, int elements) where T : unmanaged
        {
            int bytesCopied = sizeof(T) * elements;
            fixed (byte* dataPointer = _data)
            {
                Buffer.MemoryCopy(dataPointer + _offset, output, bytesCopied, bytesCopied);
                _offset += bytesCopied;
            }
        }

        /// <summary>
        /// Reads bytes as string until first instance of \0 (End of string) or until end of bytes.
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string ReadString(Encoding encoding)
        {
            byte endOfString = encoding.GetBytes("\0")[0];

            string result = null;
            for (int i = _offset; i < _data.Length; i++)
            {
                if (_data[i] == endOfString)
                {
                    fixed (byte* dataPointer = _data)
                        result = encoding.GetString(dataPointer + _offset, i - _offset);
                    _offset += i - _offset + 1;
                    return result;
                }
            }

            if (result == null)
                fixed (byte* dataPointer = _data)
                    result = encoding.GetString(dataPointer + _offset, _data.Length - _offset);

            _offset = _data.Length;
            return result;
        }

        public string ReadStringRaw(Encoding encoding)
        {
            fixed (byte* dataPointer = _data)
            {
                string result = encoding.GetString(dataPointer + _offset, _data.Length - _offset);
                _offset = _data.Length;

                return result;
            }
        }
    }
}
