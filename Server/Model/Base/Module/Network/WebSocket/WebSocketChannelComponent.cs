using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using Sining.DataStructure;
using Sining.Event;
using Sining.Module;
using Sining.Tools;

namespace Sining.Network
{
    [ComponentSystem]
    public class WebSocketChannelComponentAcceptAwakeSystem : AwakeSystem<WebSocketChannelComponent,Session,HttpListenerWebSocketContext>
    {
        protected override void Awake(WebSocketChannelComponent self, Session session, HttpListenerWebSocketContext webSocketContext)
        {
            session.Channel = self;
            self.Awake(session, webSocketContext);
        }
    }

    [ComponentSystem]
    public class WebSocketChannelComponentConnectAwakeSystem : AwakeSystem<WebSocketChannelComponent, Session, string>
    {
        protected override void Awake(WebSocketChannelComponent self, Session session, string url)
        {
            session.Channel = self;
            self.Awake(session, url);
        }
    }

    public class WebSocketChannelComponent : NetworkChannel
    {
        private HttpListenerWebSocketContext _webSocketContext;
        private WebSocket _webSocket;
        private Session _session;
        private bool _isSending;
        private PacketParser _parser;
        private readonly CircularBuffer _recvBuffer = new CircularBuffer();
        private readonly CircularBuffer _sendBuffer = new CircularBuffer();
        private CancellationTokenSource _cancellationTokenSource;

        public void Awake(Session session, HttpListenerWebSocketContext webSocketContext)
        {
            _webSocketContext = webSocketContext;
            _webSocket = _webSocketContext.WebSocket;
            RemoteAddress = $"{webSocketContext.RequestUri.Host}:{webSocketContext.RequestUri.Port}";
            MemoryStream = MemoryStreamPool.Instance.GetStream("Message", short.MaxValue);
            _session = session;
            _session.MemoryStream = MemoryStream;
            _parser = new PacketParser(_recvBuffer, MemoryStream);
            _cancellationTokenSource = new CancellationTokenSource();
            
            StartRecvAsync();
            OnConnectComplete();
        }

        public void Awake(Session session, string url)
        {
            _webSocket = new ClientWebSocket();
            MemoryStream = MemoryStreamPool.Instance.GetStream("Message", short.MaxValue);
            _session = session;
            _session.MemoryStream = MemoryStream;
            _parser = new PacketParser(_recvBuffer, MemoryStream);
            _cancellationTokenSource = new CancellationTokenSource();

            ConnectAsync(url);
        }

        private void ConnectAsync(string url)
        {
            try
            {
                ((ClientWebSocket) _webSocket).ConnectAsync(new Uri(url), _cancellationTokenSource.Token)
                    .ConfigureAwait(false).GetAwaiter().GetResult();

                StartRecvAsync();

                OnConnectComplete();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        #region Recv

        private async void StartRecvAsync()
        {
            if (IsDispose) return;

            try
            {
                for (;;)
                {
                    ValueWebSocketReceiveResult receiveResult;

                    do
                    {
                        var buffer = _recvBuffer.Last.AsMemory(_recvBuffer.LastIndex,
                            _recvBuffer.ChunkSize - _recvBuffer.LastIndex);

                        try
                        {
                            receiveResult = await _webSocket.ReceiveAsync(
                                buffer, _cancellationTokenSource.Token);
                        }
                        catch
                        {
                            OnDisconnectComplete();
                            return;
                        }

                        _recvBuffer.LastIndex += receiveResult.Count;

                        if (_recvBuffer.LastIndex >= _recvBuffer.ChunkSize)
                        {
                            _recvBuffer.AddLast();
                            _recvBuffer.LastIndex = 0;
                        }

                        if (IsDispose) return;

                    } while (!receiveResult.EndOfMessage);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        return;
                    }

                    OneThreadSynchronizationContext.Instance.Post(OnRecvComplete, null);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                Dispose();
            }
        }

        private void OnRecvComplete(object o)
        {
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
        }

        #endregion

        #region Send

        public override void Send(Session session, MemoryStream memoryStream)
        {
            if (IsDispose) return;

            _sendBuffer.Write(memoryStream);

            StartSend();
        }

        private async void StartSend()
        {
            if (IsDispose || _isSending) return;

            for (;;)
            {
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

                    _isSending = true;

                    await _webSocket.SendAsync(
                        _sendBuffer.First.AsMemory(_sendBuffer.FirstIndex, sendSize),
                        WebSocketMessageType.Binary, true,
                        _cancellationTokenSource.Token);

                    _sendBuffer.FirstIndex += sendSize;

                    if (_sendBuffer.FirstIndex == _sendBuffer.ChunkSize)
                    {
                        _sendBuffer.FirstIndex = 0;
                        _sendBuffer.RemoveFirst();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        #endregion
        private void OnConnectComplete() { }
        private void OnDisconnectComplete() => Dispose();
        public override void Dispose()
        {
            if(IsDispose) return;

            base.Dispose();
            
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            _webSocketContext = null;
            _webSocket.Dispose();
            
            _recvBuffer.Clear();
            _sendBuffer.Clear();
            _isSending = false;
            MemoryStream.Dispose();
        }
    }
}