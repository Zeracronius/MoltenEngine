using Molten.Collections;
using Molten.Net.Message;
using Molten.Threading;
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
        public string Identity { get; private set; }

        public int Port { get; private set; }
        public IPAddress LocalAddress { get; private set; }
        public ThreadedQueue<MNetRawMessage> Inbox { get; }

        private Socket _tcpListener;
        private Socket _udpListener;

        private EndPoint _udpEndpoint;

        private ThreadedQueue<byte[]> _buffers;
        private EngineThread _tcpListeningThread;
        private EngineThread _udpListeningThread;

        private WorkerGroup _workerGroup;

        public MNetListener()
        {
            _tcpListener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _udpListener = new Socket(SocketType.Dgram, ProtocolType.Udp);
            _buffers = new ThreadedQueue<byte[]>();

            // Allow UDP from any source.
            _udpEndpoint = new IPEndPoint(IPAddress.Any, Port);

            _tcpListener.Blocking = false;

            Inbox = new ThreadedQueue<MNetRawMessage>();
        }

        public void Initialize(ThreadManager threading, IPAddress localAddress, int port)
        {
            Identity = $"{localAddress}:{port}_Listener";
            LocalAddress = localAddress;
            Port = port;


            _workerGroup = threading.SpawnWorkerGroup(Identity + "_Worker", 10);

            IPEndPoint localEndPoint = new IPEndPoint(localAddress, port);
            _udpListener.Bind(localEndPoint);
            _tcpListener.Bind(localEndPoint);

            _tcpListener.Listen(100);

            _udpListeningThread = threading.SpawnThread($"{Identity}_Listener_UDP", true, false, ListenUDP);
            _tcpListeningThread = threading.SpawnThread($"{Identity}_Listener_TCP", true, false, ListenTCP);
        }

        private void ListenTCP(Timing timing)
        {
            while(_tcpListener.Poll(10, SelectMode.SelectRead))
            {
              Socket connection = _tcpListener.Accept();

              IWorkerTask task = Tasks.ReadTcpTask.Get(_buffers, connection);
              task.OnCompleted += MessageTask_OnCompleted;
              _workerGroup.QueueTask(task);
            }
        }

        private void MessageTask_OnCompleted(IWorkerTask task)
        {
            Tasks.IMessageReadTask messageTask = (Tasks.IMessageReadTask)task;
            if (messageTask.Message.HasValue)
                Inbox.Enqueue(messageTask.Message.Value);

            task.OnCompleted -= MessageTask_OnCompleted;
        }

        private void ListenUDP(Timing timing)
        {
            byte[] buffer;
            if (_buffers.TryDequeue(out buffer) == false)
                buffer = new byte[1024];

            int receivedBytes = _udpListener.ReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref _udpEndpoint);


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

            IPEndPoint remoteIP = (IPEndPoint)_udpEndpoint;
            MNetRawMessage rawMessage = new MNetRawMessage(messagePrefix, data, remoteIP.Address);
            Inbox.Enqueue(rawMessage);
        }

        public void Dispose()
        {
            _tcpListeningThread.DisposeAndJoin();
            _udpListeningThread.DisposeAndJoin();
        }
    }
}
