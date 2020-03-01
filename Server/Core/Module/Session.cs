using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Server.Network;
using Sining.Event;
using Sining.Network;
using Sining.Tools;

namespace Sining.Module
{
    [ComponentSystem]
    public class SessionAwakeSystem : AwakeSystem<Session,NetworkComponent>
    {
        protected override void Awake(Session self, NetworkComponent networkComponent)
        {
            self.Network = networkComponent;
        }
    }

    public class Session : Component
    {
        public static int RpcId;
        public long LastRecvTime { get; private set; }
        public long LastSendTime { get; private set; }

        public NetworkChannel Channel;
        public MemoryStream MemoryStream;
        public NetworkComponent Network;
        public HttpListenerContext Context;
        
        private readonly Dictionary<int,Action<IResponse>> _requestCallback = new Dictionary<int, Action<IResponse>>();

        #region Send

        public void SendToHttp(IMessage message)
        {
            try
            {
                Unpack(message);

                Context.Response.StatusCode = 200;

                Context.Response.ContentType = "application/octet-stream";

                Context.Response.OutputStream.Write(MemoryStream.GetBuffer());

                Context.Response.Close();

                Dispose();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        public void Send(IMessage message)
        {
            try
            {
                Unpack(message);
                
                Channel.Send(Channel.MemoryStream);

                LastSendTime = TimeHelper.Now;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        #endregion

        #region Call

        public STask<TResponse> Call<TResponse>(IRequest request) where TResponse : IResponse
        {
            var rpcId = ++RpcId;

            var tcs = new STaskCompletionSource<TResponse>();

            _requestCallback[rpcId] = response =>
            {
                if (response is ErrorResponse)
                {
                    tcs.SetException(new Exception($"Rpc error errorCode: {response.ErrorCode}"));
                    return;
                }

                tcs.SetResult((TResponse) response);
            };

            request.RpcId = rpcId;

            Send(request);

            return tcs.Task;
        }
        public STask<IResponse> Call(IRequest request)
        {
            var rpcId = ++RpcId;

            var tcs = new STaskCompletionSource<IResponse>();

            _requestCallback[rpcId] = response =>
            {
                if (response is ErrorResponse)
                {
                    tcs.SetException(new Exception($"Rpc error errorCode: {response.ErrorCode}"));
                    return;
                }

                tcs.SetResult(response);
            };

            request.RpcId = rpcId;

            Send(request);

            return tcs.Task;
        }

        #endregion

        #region Receive

        public void Receive(HttpListenerContext context, ushort code, MemoryStream memoryStream)
        {
            Context = context;

            Receive(code, memoryStream);
        }
        public void Receive(ushort code, MemoryStream memoryStream)
        {
            LastRecvTime = TimeHelper.Now;

            try
            {
                var message = Network.MessagePacker.DeserializeFrom(
                    NetworkProtocolManagement.Instance.GetType(code), memoryStream);

                Dispatcher(message);
            }
            catch (Exception e)
            {
                // 解析失败表示有可能是其他人攻击
                Log.Error($"code: {code} {Network.Count} {e}, ip: {Channel.RemoteAddress}");
                Dispose();
            }
        }
        
        private void Dispatcher(object message)
        {
            try
            {
                if (message is IResponse response)
                {
                    // 如果是回调消息，执行消息的回调

                    if (!_requestCallback.TryGetValue(response.RpcId, out var action))
                    {
                        throw new Exception($"not found rpc, response message: {response.GetType().Name}");
                    }
                    
                    _requestCallback.Remove(response.RpcId);

                    action(response);

                    return;
                }

                MessageDispatcherManagement.Instance.Handle(this, message);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        #endregion
        
        private void Unpack(IMessage message)
        {
            if (message == null)
            {
                throw new Exception("message cannot be null");
            }

            if (IsDispose)
            {
                throw new Exception("session has been Disposed");
            }

            var opCode = NetworkProtocolManagement.Instance.GetOpCode(message.GetType());

            MemoryStream.Seek(PacketParser.PacketHeadLength, SeekOrigin.Begin);
            MemoryStream.SetLength(PacketParser.PacketHeadLength);
            Network.MessagePacker.SerializeTo(message, MemoryStream);

            var byteLength = MemoryStream.Length - PacketParser.PacketHeadLength;
            if (byteLength > PacketParser.PacketBody)
            {
                throw new Exception($"Message content exceeds {PacketParser.PacketBody} bytes");
            }

            MemoryStream.Seek(0, SeekOrigin.Begin);
            MemoryStream.Write(BitConverter.GetBytes((int) byteLength));
            MemoryStream.Write(BitConverter.GetBytes(opCode));
            MemoryStream.Seek(0, SeekOrigin.Begin);
        }
        public override void Dispose()
        {
            if (IsDispose) return;

            foreach (var action in _requestCallback.Values.ToArray())
            {
                action(new ErrorResponse {ErrorCode = ErrorCode.ErrSessionDispose});
            }

            _requestCallback.Clear();

            if (Channel != null && !Channel.IsDispose)
            {
                Channel.Dispose();
            }
            
            Context = null;

            base.Dispose();

            LastRecvTime = 0;
            LastSendTime = 0;
            Network = null;
            MemoryStream = null;
            Channel = null;
            Context = null;
        }
    }
}