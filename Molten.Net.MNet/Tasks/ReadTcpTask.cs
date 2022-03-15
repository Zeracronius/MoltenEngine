using Molten.Collections;
using Molten.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Net.MNet.Tasks
{
    internal class ReadTcpTask : MessageReadTask, IPoolable
    {
        static ObjectPool<ReadTcpTask> _pool = new ObjectPool<ReadTcpTask>(() => new ReadTcpTask());
        internal static ReadTcpTask Get(ThreadedQueue<byte[]> buffers, Socket connection)
        {
            ReadTcpTask task = _pool.GetInstance();
            task._buffers = buffers;
            task._connection = connection;
            return task;
        }

        private Socket? _connection;
        private ThreadedQueue<byte[]>? _buffers;

        protected override bool OnRun()
        {
            if (_connection == null)
                return false;

            _connection.Blocking = true;
            EndPoint remoteEndpoint = _connection.RemoteEndPoint;
            Queue<byte[]> filled = new Queue<byte[]>();
            int bytesRecieved = 0;
            int totalBytes = 0;
            byte[] buffer;
            do
            {
                if (_buffers.TryDequeue(out buffer) == false)
                    buffer = new byte[1024];

                bytesRecieved = _connection.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                if (bytesRecieved == 0)
                    break;

                totalBytes += bytesRecieved;
                filled.Enqueue(buffer);

                if (bytesRecieved < buffer.Length)
                    break;
            }
            while (buffer[bytesRecieved] != 0x1A && _connection.Connected);
            
            _connection.Shutdown(SocketShutdown.Both);
            _connection.Disconnect(true);

            int prefix = MessagePrefix.Size;
            byte[] data = new byte[totalBytes - prefix];
            int position = 0;
            MessagePrefix messagePrefix = new MessagePrefix();
            while (filled.Count > 0)
            {
                byte[] usedBuffer = filled.Dequeue();
                int remainingBytes = Math.Min(totalBytes - prefix - position, 1024);

                Array.Copy(buffer, prefix, data, position, remainingBytes);
                position += usedBuffer.Length;

                // Read prefix
                if (prefix > 0)
                {
                    unsafe
                    {
                        MessagePrefix* prefixPointer = &messagePrefix;
                        fixed (byte* bufferPointer = buffer)
                            Buffer.MemoryCopy(bufferPointer, prefixPointer, MessagePrefix.Size, MessagePrefix.Size);
                    }
                    prefix = 0;
                }

                _buffers.Enqueue(buffer);
            }

            IPEndPoint remoteIP = (IPEndPoint)remoteEndpoint;
            Message = new MNetRawMessage(messagePrefix, data, remoteIP.Address);

            _connection.Dispose();
            return true;
        }

        protected override void OnFree()
        {
            _pool.Recycle(this);
        }

        public void ClearForPool()
        {
            _connection = null;
            _buffers = null;
            Message = null;
        }
    }
}
