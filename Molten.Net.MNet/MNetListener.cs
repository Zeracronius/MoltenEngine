using Molten.Collections;
using Molten.Net.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Molten.Net.MNet
{
    internal class MNetListener : IDisposable
    {
        public int Port { get; private set; }
        public IPAddress LocalAddress { get; private set; }
        public ThreadedQueue<MNetRawMessage> Inbox { get; }

        private Socket _tcpListener;
        private Socket _udpListener;

        private ThreadedQueue<byte[]> _buffers;
        private Thread _tcpListeningThread;
        private Thread _udpListeningThread;

        public MNetListener()
        {
            _tcpListener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _udpListener = new Socket(SocketType.Dgram, ProtocolType.Udp);
            _buffers = new ThreadedQueue<byte[]>();

            Inbox = new ThreadedQueue<MNetRawMessage>();
        }

        public void Initialize(IPAddress localAddress, int port)
        {
            LocalAddress = localAddress;
            Port = port;

            IPEndPoint localEndPoint = new IPEndPoint(localAddress, port);
            _udpListener.Bind(localEndPoint);
            _tcpListener.Bind(localEndPoint);

            _tcpListener.Listen(100);

            _udpListeningThread = new Thread(ListenUDP);
            _udpListeningThread.Start();

            _tcpListeningThread = new Thread(ListenTCP);
            _tcpListeningThread.Start();
        }

        private void ListenTCP()
        {
            while (true)
            {
                using (Socket connection = _tcpListener.Accept())
                {
                    EndPoint remoteEndpoint = connection.RemoteEndPoint;

                    Queue<byte[]> filled = new Queue<byte[]>();
                    int bytesRecieved = 0;
                    int totalBytes = 0;
                    byte[] buffer;
                    do
                    {
                        if (_buffers.TryDequeue(out buffer) == false)
                            buffer = new byte[1024];

                        bytesRecieved = connection.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                        if (bytesRecieved == 0)
                            break;

                        totalBytes += bytesRecieved;
                        filled.Enqueue(buffer);

                        if (bytesRecieved < buffer.Length)
                            break;
                    }
                    while (buffer[bytesRecieved] != 0x1A);

                    connection.Disconnect(true);

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

                    IPEndPoint remoteIP = connection.RemoteEndPoint as IPEndPoint;
                    MNetRawMessage rawMessage = new MNetRawMessage(messagePrefix, data, remoteIP.Address);
                    Inbox.Enqueue(rawMessage);
                }
            }
        }

        private void ListenUDP()
        {
            while (true)
            {
                byte[] buffer;
                if (_buffers.TryDequeue(out buffer) == false)
                    buffer = new byte[1024];

                EndPoint endpoint = new IPEndPoint(IPAddress.Any, Port);
                int receivedBytes = _udpListener.ReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endpoint);


                byte[] data = new byte[receivedBytes - MessagePrefix.Size];

                // Read prefix
                MessagePrefix messagePrefix;
                unsafe
                {
                    MessagePrefix* prefixPointer = &messagePrefix;
                    fixed (byte* bufferPointer = buffer)
                        Buffer.MemoryCopy(bufferPointer, prefixPointer, MessagePrefix.Size, MessagePrefix.Size);
                }
                Array.Copy(buffer, MessagePrefix.Size, data, 0, data.Length);

                _buffers.Enqueue(buffer);

                IPEndPoint remoteIP = endpoint as IPEndPoint;
                MNetRawMessage rawMessage = new MNetRawMessage(messagePrefix, data, remoteIP.Address);
                Inbox.Enqueue(rawMessage);
            }
        }

        public void Dispose()
        {
            _tcpListeningThread.Abort();
            _udpListeningThread.Abort();
        }
    }
}
