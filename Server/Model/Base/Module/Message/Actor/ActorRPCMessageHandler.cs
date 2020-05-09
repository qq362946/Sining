using System;
using Sining.Module;

namespace Sining.Network.Actor
{
    public abstract class ActorRPCMessageHandler<TComponent, TActorRequest, TActorResponse> : IActorMessageHandler
        where TComponent : Component
        where TActorRequest : class, IActorRequest
        where TActorResponse : class, IActorResponse
    {
        public Type Type() => typeof(TActorRequest);

        protected abstract STask Run(TComponent component, TActorRequest actorRequest, TActorResponse actorResponse,
            Action reply);

        public async STask Handle(Session session, Component component, object message)
        {
            if (!(message is TActorRequest actorRequest))
            {
                Log.Error($"消息类型转换错误: {message.GetType().FullName} to {typeof(TActorRequest).Name}");
                return;
            }

            if (!(component is TComponent tComponent))
            {
                Log.Error($"Actor类型转换错误: {component.GetType().Name} to {typeof(TComponent).Name}");
                return;
            }
            
            var rpcId = actorRequest.RpcId;
            var sessionInstanceId = session.InstanceId;
            var actorResponse = Activator.CreateInstance<TActorResponse>();
            var isReply = false;
            
            void Reply()
            {
                if(isReply) return;
                
                isReply = true;
            
                if (session.InstanceId != sessionInstanceId) return;

                actorResponse.RpcId = rpcId;
                session.Send(actorResponse);
            }
            
            try
            {
                await Run(tComponent, actorRequest, actorResponse, Reply);
            }
            catch (Exception e)
            {
                Log.Error(e);
                actorResponse.ErrorCode = ModelErrorCode.ErrRpcFail;
            }
            finally
            {
                Reply();
            }
        }
    }
}