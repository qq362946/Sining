using System;
using Sining.Module;

namespace Sining.Network
{
    public abstract class MessageHandler<TMessage> : IMessageHandler
    {
        protected abstract void Run(Session session, TMessage message);

        public Type Type() => typeof(TMessage);

        public async void Handle(Session session, object msg)
        {
            if (session.IsDispose)
            {
                Log.Info($"session disconnect {msg}");
                return;
            }

            try
            {
                Run(session, (TMessage) msg);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            await STask.CompletedTask;
        }
    }
}