using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten
{
    public unsafe class DataWriter
    {
        byte[] _bytes;
        int _bytePosition;

        public DataWriter()
        {
            _bytes = new byte[20];
            _bytePosition = 0;
        }

        public void Clear()
        {
            _bytePosition = 0;
        }

        private void EnsureCapacity(int requiredFreeSpace)
        {
            if (_bytes.Length - _bytePosition < requiredFreeSpace)
            {
                byte[] newArray = new byte[_bytes.Length * 2 + requiredFreeSpace];
                _bytes.CopyTo(newArray, 0);
                _bytes = newArray;
            }
        }


        public void WriteString(string text, Encoding encoding)
        {
            WriteStringRaw(text + "\0", encoding);
        }

        public void WriteStringRaw(string text, Encoding encoding)
        {
            byte[] textBytes = encoding.GetBytes(text);
            fixed (byte* bytes = textBytes)
                Write(bytes, textBytes.Length);
        }

        public void Write<T>(T[] data) where T : unmanaged
        {
            int valueSize = sizeof(T) * data.Length;
            fixed (T* bytes = data)
                Write(bytes, valueSize);
        }

        public void Write<T>(T value) where T : unmanaged
        {
            Write<T>(&value);
        }

        public void Write<T>(T* valuePointer) where T : unmanaged
        {
            int valueSize = sizeof(T);
            Write(valuePointer, valueSize);
        }

        private void Write(void* valuePointer, int valueSize)
        {
            EnsureCapacity(valueSize);
            fixed (byte* bytesPointer = _bytes)
            {
                Buffer.MemoryCopy(valuePointer, bytesPointer + _bytePosition, _bytes.Length - _bytePosition, valueSize);
                _bytePosition += valueSize;
            }
        }

        /// <summary>
        /// Create a new array with no excess slots.
        /// </summary>
        /// <returns></returns>
        public byte[] GetData()
        {
            // Create a new array with no excess slots.
            byte[] output = new byte[_bytePosition];
            Array.Copy(_bytes, output, _bytePosition);
            return output;

        }
    }
}
