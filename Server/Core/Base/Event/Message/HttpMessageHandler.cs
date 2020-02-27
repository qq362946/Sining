using System;
using Sining.Module;

namespace Sining.Network
{
    public abstract class HttpMessageHandler<TRequest, TResponse>:
        IMessageHandler
        where TRequest : class, IRequest
        where TResponse : class, IResponse
    {
        private int _rpcId;
        private TResponse _response;
        private Session _session;
        /// <summary>
        /// 是否自动回复RPC消息，如果为false需要手动调用Reply()方法
        /// </summary>
        private bool _isReply;
        
        public Type Type() => typeof(TRequest);
        
        protected abstract void Run(Session session, TRequest request, TResponse response);

        public void Handle(Session session, object message)
        {
            if (!(message is TRequest request))
            {
                Log.Error($"消息类型转换错误: {message.GetType().Name} to {typeof(TRequest).Name}");

                return;
            }

            _rpcId = request.RpcId;
            _session = session;
            _response = Activator.CreateInstance<TResponse>();
            _isReply = true;

            try
            {
                Run(session, request, _response);
            }
            catch (Exception e)
            {
                Log.Error(e);
                _response.ErrorCode = ErrorCode.ErrRpcFail;
            }
            finally
            {
                if (_isReply) Reply();
            }
        }

        protected void Reply()
        {
            _isReply = false;

            _response.RpcId = _rpcId;
            _session.SendToHttp(_response);
        }
    }
}