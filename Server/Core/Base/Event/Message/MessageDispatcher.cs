using System;
using Sining.DataStructure;
using Sining.Module;

namespace Sining.Network
{
    public static class MessageDispatcher
    {
        private static readonly OneToManyList<Type, IMessageHandler> Handlers =
            new OneToManyList<Type, IMessageHandler>(0);
        
        public static void AddHandler(Type type,IMessageHandler message)
        {
            Handlers.Add(type, message);
        }

        public static void Clear()
        {
            Handlers.Clear();
        }

        public static void Handle(Session session, object obj)
        {
            if (!Handlers.TryGetValue(obj.GetType(), out var list))
            {
                Log.Warning("The message was not processed");
                return;
            }

            foreach (var messageHandler in list)
            {
                messageHandler.Handle(session, obj);
            }
        }
    }
}