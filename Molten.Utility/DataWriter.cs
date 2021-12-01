using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten
{
    internal unsafe struct DataWriter
    {
        byte[] _bytes;
        int _bytePosition;
        Encoding _encoding;

        public DataWriter()
        {
            _bytes = new byte[20];
            _bytePosition = 0;
            _encoding = Encoding.UTF8;
        }

        public DataWriter(Encoding encoding)
            : this()
        {
            _encoding = encoding;
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

        public void Write(string text)
        {
            byte[] textBytes = _encoding.GetBytes(text);

            EnsureCapacity(textBytes.Length);
            fixed (byte* bytesPointer = _bytes)
            {
                fixed (byte* textPointer = textBytes)
                    Buffer.MemoryCopy(textPointer, bytesPointer, _bytePosition, textBytes.Length);
            }
        }

        public void Write<T>(T value) where T : unmanaged
        {
            Write<T>(&value);
        }

        public void Write<T>(T* valuePointer) where T : unmanaged
        {
            int valueSize = sizeof(T);
            fixed (byte* ptrArray = _bytes)
            {
                Buffer.MemoryCopy(valuePointer, ptrArray, _bytePosition, valueSize);
                _bytePosition += valueSize;
            }
        }

        public byte[] GetData()
        {
            // Create a new array with no excess slots.
            byte[] output = new byte[_bytePosition];
            Array.Copy(_bytes, output, _bytePosition);
            return output;

        }
    }
}
