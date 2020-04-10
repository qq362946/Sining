using Sining.Module;
using Sining.Network.Actor;

namespace Sining.Network
{
    public class OuterMessageForwardDispatcher : IMessageDispatcher
    {
        public async SVoid Dispatch(Session session, ushort code, object message)
        {
            switch (message)
            {
                case IActorRequest actorRequest:  
                {
                    break;
                }
                case IActorMessage actorMessage:  
                {
                    break;
                }
                default:
                {
                    break;
                }
            }
        }
    }
}