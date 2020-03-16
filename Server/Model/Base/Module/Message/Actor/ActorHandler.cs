using System;
using Sining.Module;

namespace Sining.Network.Actor
{
    public interface IActorMessageHandler
    {
        public Type Type();
        
        STask Handle(Session session, Component component, object message);
    }
}