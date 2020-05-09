using System;
using Sining.Module;

namespace Sining.Network
{
    public abstract class RPCSceneMessageHandler<TRequest, TResponse>
        : IMessageHandler
        where TRequest : class, IRequest
        where TResponse : class, IResponse
    {
        public Type Type() => typeof(TRequest);

        protected abstract STask Run(Session session, Scene scene, TRequest request, TResponse response,
            Action reply);

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
                if (request.SceneId == 0)
                {
                    response.ErrorCode = ModelErrorCode.RequestSceneIdIsZero;
                    
                    throw new Exception("SceneId Is Zero!");
                }

                var scene = SceneManagementComponent.Instance.GetScene(request.SceneId);

                await Run(session, scene, request, response, Reply);
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