using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten
{
    internal class DataPacker
    {
        List<byte> _bytes;
        Encoding _encoding;

        public DataPacker()
        {
            _bytes = new List<byte>();
            _encoding = Encoding.UTF8;
        }

        public DataPacker(Encoding encoding)
            : this()
        {
            _encoding = encoding;
        }



        public void Write(string text)
        {
            _bytes.AddRange(_encoding.GetBytes(text));
        }

        public void Write(byte value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(sbyte value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(short value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(ushort value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(int value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(uint value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(ulong value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(float value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(double value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }


        public byte[] GetData()
        {
            return _bytes.ToArray();
        }

        public void Clear()
        {
            _bytes.Clear();
        }
    }
}
