using Sining.Module;
using Sining.Network.Actor;

namespace Sining.Network
{
    public class OuterMessageDispatcher : IMessageDispatcher
    {
        public async SVoid Dispatch(Session session, ushort code, object message)
        {
            await STask.CompletedTask;
            // switch (message)
            // {
            //     case IActorRequest actorRequest:  
            //     {
            //         ActorDispatcherComponent.Instance.Handle();
            //         break;
            //     }
            //     case IActorMessage actorMessage:  
            //     {
            //         break;
            //     }
            //     default:
            //     {
            //         break;
            //     }
            // }
        }
    }
}