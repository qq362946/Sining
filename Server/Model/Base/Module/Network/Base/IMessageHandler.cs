using System;
using Sining.Module;

namespace Sining.Network
{
    public interface IMessageHandler
    {
        public Type Type();
        
        STask Handle(Session session, object message);
    }
}