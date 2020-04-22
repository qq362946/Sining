using System;
using Sining.Module;

namespace Sining.Network.Actor
{
    public abstract class ActorMessageMessageHandler<TComponent, TMessage> : IActorMessageHandler
        where TComponent : Component
        where TMessage : class, IActorMessage
    {
        public Type Type() => typeof(TMessage);

        protected abstract STask Run(TComponent component, TMessage message);

        public async STask Handle(Session session, Component component, object message)
        {
            if (!(message is TMessage actorMessage))
            {
                Log.Error($"消息类型转换错误: {message.GetType().FullName} to {typeof(TMessage).Name}");
                return;
            }

            if (!(component is TComponent tComponent))
            {
                Log.Error($"Actor类型转换错误: {component.GetType().Name} to {typeof(TComponent).Name}");
                return;
            }

            try
            {
                await Run(tComponent, actorMessage);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }
    }
}