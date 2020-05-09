using System;
using Sining.Module;

namespace Sining.Network
{
    public abstract class RPCMessageHandler<TRequest, TResponse> :
        IMessageHandler
        where TRequest : class, IRequest
        where TResponse : class, IResponse
    {
        public Type Type() => typeof(TRequest);

        protected abstract STask Run(Session session, TRequest request, TResponse response,Action reply);

        public async STask Handle(Session session, object message)
        {
            if (!(message is TRequest request))
            {
                Log.Error($"消息类型转换错误: {message.GetType().Name} to {typeof(TRequest).Name}");

                return;
            }

            var rpcId = request.RpcId;
            var sessionInstanceId = session.InstanceId;
            var response = Activator.CreateInstance<TResponse>();
            var isReply = false;

            void Reply()
            {
                if (isReply) return;

                isReply = true;

                if (session.InstanceId != sessionInstanceId) return;

                response.RpcId = rpcId;
                session.Send(response);
            }

            try
            {
                await Run(session, request, response, Reply);
            }
            catch (Exception e)
            {
                Log.Error(e);
                response.ErrorCode = ModelErrorCode.ErrRpcFail;
            }
            finally
            {
                Reply();
            }
        }
    }
}