using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Sining.DataStructure;
using Sining.Event;
using Sining.Module;
using Sining.Tools;

namespace Sining.Network
{
    [ComponentSystem]
    public class TCPChannelComponentConnectAwakeSystem : AwakeSystem<TCPChannelComponent, Session, IPEndPoint>
    {
        protected override void Awake(TCPChannelComponent self, Session session, IPEndPoint ipEndPoint)
        {
            session.Channel = self;
            self.Awake(session, ipEndPoint);
        }
    }

    [ComponentSystem]
    public class TCPChannelComponentAwakeSystem : AwakeSystem<TCPChannelComponent, Session, SocketAsyncEventArgs>
    {
        protected override void Awake(TCPChannelComponent self, Session session, SocketAsyncEventArgs asyncEventArgs)
        {
            session.Channel = self;
            self.Awake(session, asyncEventArgs);
        }
    }

    public class TCPChannelComponent : NetworkChannel
    {
        private Socket _socket;
        private readonly SocketAsyncEventArgs _innArgs = new SocketAsyncEventArgs();
        private readonly SocketAsyncEventArgs _outArgs = new SocketAsyncEventArgs();
        private readonly CircularBuffer _recvBuffer = new CircularBuffer();
        private readonly CircularBuffer _sendBuffer = new CircularBuffer();
        private Session _session;
        private EndPoint _remoteIpEndPoint;
        private PacketParser _parser;
        private bool _isSending;

        #region Awake

        public void Awake(Session session, SocketAsyncEventArgs asyncEventArgs)
        {
            _socket = asyncEventArgs.AcceptSocket;
            _socket.NoDelay = true;

            MemoryStream = MemoryStreamPool.Instance.GetStream("Message", short.MaxValue);
            _session = session;
            _parser = new PacketParser(_recvBuffer, MemoryStream);
            _innArgs.Completed += OnComplete;
            _outArgs.Completed += OnComplete;
            
            _remoteIpEndPoint = asyncEventArgs.RemoteEndPoint;
            RemoteAddress = _socket.RemoteEndPoint.ToString();

            OnConnectComplete(asyncEventArgs);
        }

        public void Awake(Session session, IPEndPoint ipEndPoint)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {NoDelay = true};

            MemoryStream = MemoryStreamPool.Instance.GetStream("Message", short.MaxValue);

            _session = session;
            _parser = new PacketParser(_recvBuffer, MemoryStream);
            _innArgs.Completed += OnComplete;
            _outArgs.Completed += OnComplete;
            
            RemoteAddress = ipEndPoint.ToString();
            _remoteIpEndPoint = ipEndPoint;

            ConnectAsync(_remoteIpEndPoint);
        }

        #endregion

        #region Connect

        private void ConnectAsync(EndPoint ipEndPoint)
        {
            var outArgs = new SocketAsyncEventArgs {RemoteEndPoint = ipEndPoint};
            
            outArgs.Completed += OnComplete;

            if (_socket.ConnectAsync(outArgs))
            {
                return;
            }

            OnConnectComplete(outArgs);
        }

        private void OnConnectComplete(object o)
        {
            if (IsDispose) return;

            var asyncEventArgs = (SocketAsyncEventArgs) o;

            if (asyncEventArgs.SocketError != SocketError.Success)
            {
                return;
            }

            StartRecvAsync();
        }

        #endregion

        #region Recv

        private void StartRecvAsync()
        {
            RecvAsync(_recvBuffer.Last,
                _recvBuffer.LastIndex,
                _recvBuffer.ChunkSize - _recvBuffer.LastIndex);
        }

        private void RecvAsync(byte[] buffer, int offset, int count)
        {
            if (IsDispose) return;

            try
            {
                _innArgs.SetBuffer(buffer, offset, count);
            }
            catch (Exception e)
            {
                throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
            }

            if (_socket.ReceiveAsync(_innArgs)) return;

            OnRecvComplete(_innArgs);
        }

        private void OnRecvComplete(object o)
        {
            if (IsDispose) return;

            var asyncEventArgs = (SocketAsyncEventArgs) o;

            if (asyncEventArgs.SocketError != SocketError.Success) return;

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
                _recvBuffer.LastIndex += asyncEventArgs.BytesTransferred;

                if (_recvBuffer.LastIndex >= _recvBuffer.ChunkSize)
                {
                    _recvBuffer.AddLast();
                    _recvBuffer.LastIndex = 0;
                }

                for (;;)
                {
                    try
                    {
                        if (!_parser.Parse()) break;
                    }
                    catch (Exception e)
                    {
                        // 出现这样错误，肯定是其他人恶意发送封包造成的。
                        // 所以直接断开该用户的连接是最好的选择。

                        Log.Warning($"Likely to be attacked IP: {RemoteAddress} {e}");

                        Dispose();

                        return;
                    }

                    try
                    {
                        _session.Receive(_parser.MessageProtocolCode, MemoryStream);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                    finally
                    {
                        _parser.Clear();
                    }
                }

                StartRecvAsync();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        #endregion

        #region Send

        private void StartSend()
        {
            if (IsDispose || _isSending) return;
            
            _isSending = true;

            if (_sendBuffer.Length == 0)
            {
                _isSending = false;
                return;
            }
            try
            {
                var sendSize = _sendBuffer.ChunkSize - _sendBuffer.FirstIndex;
                if (sendSize > _sendBuffer.Length)
                {
                    sendSize = (int) _sendBuffer.Length;
                }

                SendAsync(_sendBuffer.First, _sendBuffer.FirstIndex, sendSize);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void SendAsync(byte[] buffer, int offset, int count)
        {
            if (IsDispose) return;

            try
            {
                _outArgs.SetBuffer(buffer, offset, count);
            }
            catch (Exception e)
            {
                throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
            }
        
            if (_socket.SendAsync(_outArgs))
            {
                return;
            }
        
            OnSendComplete(_outArgs);
        }

        public override void Send(Session session, MemoryStream memoryStream)
        {
            if (IsDispose) return;

            _sendBuffer.Write(memoryStream);

            StartSend();
        }

        private void OnSendComplete(object o)
        {
            if (_socket == null || IsDispose) return;

            var asyncEventArgs = (SocketAsyncEventArgs) o;

            if (asyncEventArgs.SocketError != SocketError.Success)
            {
                return;
            }

            if (asyncEventArgs.BytesTransferred == 0)
            {
                return;
            }

            _sendBuffer.FirstIndex += asyncEventArgs.BytesTransferred;

            if (_sendBuffer.FirstIndex == _sendBuffer.ChunkSize)
            {
                _sendBuffer.FirstIndex = 0;
                _sendBuffer.RemoveFirst();
            }

            _isSending = false;

            StartSend();
        }

        #endregion

        private void OnComplete(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            if (IsDispose) return;

            switch (asyncEventArgs.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    OneThreadSynchronizationContext.Instance.Post(OnConnectComplete, asyncEventArgs);
                    break;
                case SocketAsyncOperation.Receive:
                    OneThreadSynchronizationContext.Instance.Post(OnRecvComplete,asyncEventArgs);
                    break;
                case SocketAsyncOperation.Send:
                    OneThreadSynchronizationContext.Instance.Post(OnSendComplete, asyncEventArgs);
                    break;
                case SocketAsyncOperation.Disconnect:
                    OneThreadSynchronizationContext.Instance.Post(OnDisconnectComplete, asyncEventArgs);
                    break;
                default:
                    throw new Exception($"Socket Error: {asyncEventArgs.LastOperation}");
            }
        }

        private void OnDisconnectComplete(object o)
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            base.Dispose();

            _innArgs.Completed -= OnComplete;
            _outArgs.Completed -= OnComplete;
            
            _recvBuffer.Clear();
            _sendBuffer.Clear();
            _socket?.Dispose();
            _socket = null;
            RemoteAddress = null;
            _remoteIpEndPoint = null;
            _isSending = false;

            MemoryStream.Dispose();

            if (_session.IsDispose) return;

            _session.Dispose();
            _session = null;
        }
    }
}