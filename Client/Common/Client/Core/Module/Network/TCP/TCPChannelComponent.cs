using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Sining.DataStructure;
using Sining.Event;
using Sining.Module;
using Sining.Tools;

namespace Sining.Network
{
    [ComponentSystem]
    public class
        TCPChannelComponentAwakeSystem : AwakeSystem<TCPChannelComponent, NetworkComponent, TCPComponent,
            SocketAsyncEventArgs>
    {
        protected override void Awake(TCPChannelComponent self, NetworkComponent networkComponent,
            TCPComponent tcpComponent,
            SocketAsyncEventArgs asyncEventArgs)
        {
            self.Awake(networkComponent, tcpComponent, asyncEventArgs);
        }
    }

    public class TCPChannelComponent : NetworkChannel
    {
        private Socket _socket;
        private readonly SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        private readonly SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        
        private readonly CircularBuffer _recvBuffer = new CircularBuffer();
        private readonly CircularBuffer _sendBuffer = new CircularBuffer();
        public string RemoteAddress { get; private set; }
        private Session _session;
        private TCPComponent _tcpComponent;

        public void Awake(NetworkComponent networkComponent, TCPComponent tcpComponent,
            SocketAsyncEventArgs asyncEventArgs)
        {
            _session = networkComponent.Create(this);
            _tcpComponent = tcpComponent;
            _socket = asyncEventArgs.AcceptSocket;
            _socket.NoDelay = true;
            
            MemoryStream = MemoryStreamPool.Instance.GetStream("Message", short.MaxValue);

            _recvArgs.Completed += OnComplete;
            _sendArgs.Completed += OnComplete;
            RemoteAddress = _socket.RemoteEndPoint.ToString();

            OnConnectComplete(asyncEventArgs);

            Start();
        }

        public override void Start()
        {
            StartRecvAsync();
        }
        
        private void StartRecvAsync()
        {
            RecvAsync(_recvBuffer.Last,
                _recvBuffer.LastIndex,
                _recvBuffer.ChunkSize - _recvBuffer.LastIndex);
        }
        
        private void RecvAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                _recvArgs.SetBuffer(buffer, offset, count);
            }
            catch (Exception e)
            {
                throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
            }
            
            if(_socket.ReceiveAsync(_recvArgs)) return;

            OnRecvComplete(_recvArgs);
        }

        private void OnRecvComplete(SocketAsyncEventArgs asyncEventArgs)
        {
            if (IsDispose)
            {
                return;
            }

            if (asyncEventArgs.SocketError != SocketError.Success)
            {
                return;
            }

            if (asyncEventArgs.BytesTransferred == 0 && !IsDispose)
            {
                OnDisconnectComplete(asyncEventArgs);
                return;
            }

            if (_socket == null)
            {
                OnDisconnectComplete(asyncEventArgs);
                return;
            }

            try
            {
                this.Lock();

                _recvBuffer.LastIndex += asyncEventArgs.BytesTransferred;

                if (_recvBuffer.LastIndex == _recvBuffer.ChunkSize)
                {
                    _recvBuffer.AddLast();
                    _recvBuffer.LastIndex = 0;
                }

                
                
                byte[] bytes = MemoryStream.GetBuffer(); 
                
                _recvBuffer.Read(bytes, 0, asyncEventArgs.BytesTransferred);
                
                _session.Receive(Encoding.UTF8.GetString(bytes));

                StartRecvAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                this.UnLock();
            }
        }

        private void OnComplete(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            if (IsDispose) return;

            switch (asyncEventArgs.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    OnConnectComplete(asyncEventArgs);
                    break;
                case SocketAsyncOperation.Receive:
                    OnRecvComplete(asyncEventArgs);
                    break;
                // case SocketAsyncOperation.Send:
                //     OneThreadSynchronizationContext.Instance.Post(this.OnSendComplete, e);
                //     break;
                case SocketAsyncOperation.Disconnect:
                    OnDisconnectComplete(asyncEventArgs);
                    break;
                default:
                    throw new Exception($"Socket Error: {asyncEventArgs.LastOperation}");
            }
        }

        private void OnConnectComplete(SocketAsyncEventArgs asyncEventArgs)
        {
            if (asyncEventArgs.SocketError != SocketError.Success)
            {
                return;
            }

            Console.WriteLine($"收到一个连接完成{RemoteAddress} ThreadId:{Thread.CurrentThread.ManagedThreadId}");
        }
        
        private void OnDisconnectComplete(SocketAsyncEventArgs asyncEventArgs)
        {
            Dispose();

            Console.WriteLine($"收到一个连接断开:{RemoteAddress}  ThreadId:{Thread.CurrentThread.ManagedThreadId}");
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            _tcpComponent.RemoveChannel(InstanceId);

            base.Dispose();

            _recvArgs.Completed -= OnComplete;
            _sendArgs.Completed -= OnComplete;
            
            _session = null;
            _recvBuffer.Clear();
            _sendBuffer.Clear();
            _socket.Dispose();
            _socket = null;
            MemoryStream.Dispose();
        }
    }
}