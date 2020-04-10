using System;
using Sining.Module;
using Sining.Network.Actor;

namespace Sining.Network
{
    public class OuterMessageDispatcher : IMessageDispatcher
    {
        public async SVoid Dispatch(Session session, ushort code, object message)
        {
            if (message is IActorMessage iActorMessage)
            {
                if (iActorMessage.ActorId == 0)
                {
                    throw new Exception("ActorId is 0");
                }

                var component = ComponentManagement.Instance.Get(iActorMessage.ActorId);

                await ActorDispatcherComponent.Instance.Handle(session, component, message);

                return;
            }

            await MessageDispatcherManagement.Instance.Handle(session, message);
        }
    }
}