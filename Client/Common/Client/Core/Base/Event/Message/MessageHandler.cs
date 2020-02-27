using System;
using Sining.Module;

namespace Sining.Network
{
    public abstract class MessageHandler<TMessage>  : IMessageHandler
    {
        protected abstract void Run(Session session, TMessage message);

        public void Handle(Session session, object msg)
        {
            if (session.IsDispose)
            {
                throw new Exception($"session disconnect {msg}");
            }

            try
            {
                Run(session, (TMessage) msg);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}