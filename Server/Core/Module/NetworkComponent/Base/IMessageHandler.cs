using System;
using Sining.Module;

namespace Sining.Network
{
    public interface IMessageHandler
    {
        public Type Type();
        
        void Handle(Session session, object message);
    }
}