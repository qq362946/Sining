using System;
using Sining.Module;

namespace Sining.Network
{
    public abstract class MessageHandler<TMessage> : IMessageHandler
    {
        protected abstract STask Run(Session session, TMessage message);

        public Type Type() => typeof(TMessage);

        public async STask Handle(Session session, object msg)
        {
            if (session.IsDispose)
            {
                Log.Info($"session disconnect {msg}");
                return;
            }

            try
            {
                await Run(session, (TMessage) msg);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            await STask.CompletedTask;
        }
    }
}