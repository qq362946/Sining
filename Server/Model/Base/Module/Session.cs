using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class SessionAwakeSystem : AwakeSystem<Session, NetworkComponent>
    {
        protected override void Awake(Session self, NetworkComponent networkComponent)
        {
            self.Network = networkComponent;
        }
    }

    public class Session : Component
    {
        private static int __rpcId;
        public long LastRecvTime { get; private set; }
        public long LastSendTime { get; private set; }
        public NetworkChannel Channel;
        public NetworkComponent Network;

        private readonly Dictionary<int, Action<IResponse>>
            _requestCallback = new Dictionary<int, Action<IResponse>>();

        public void Send(IMessage message)
        {
            try
            {
                Network.MessagePacker.Unpack(this, message, Network, ref Channel.MemoryStream);

                Channel.Send(this, Channel.MemoryStream);

                LastSendTime = TimeHelper.Now;
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        #region Call

        public STask<TResponse> Call<TResponse>(IRequest request) where TResponse : IResponse
        {
            var rpcId = ++__rpcId;

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

        #endregion

        #region Receive

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Receive(ushort code, MemoryStream memoryStream)
        {
            if (IsDispose) return;

            LastRecvTime = TimeHelper.Now;
            object message;

            try
            {
                message = Network.MessagePacker.DeserializeFrom(
                    NetworkProtocolManagement.Instance.GetType(code), memoryStream);
            }
            catch (Exception e)
            {
                // 解析失败表示有可能是其他人攻击
                Log.Error($"code: {code} {Network.Count} {e}, ip: {Channel.RemoteAddress}");
                Dispose();
                return;
            }

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
                }
                else
                {
                    Network.MessageDispatcher.Dispatch(this, code, message).Coroutine();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        #endregion
        
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

            base.Dispose();

            LastRecvTime = 0;
            LastSendTime = 0;
            Network = null;
            Channel = null;
        }
    }
}