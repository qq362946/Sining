using System;
using Sining.DataStructure;
using Sining.Module;

namespace Sining.Network
{
    public static class MessageDispatcher
    {
        private static readonly OneToManyList<ushort, IMessageHandler> Handlers =
            new OneToManyList<ushort, IMessageHandler>(0);
        
        public static void AddHandler(IMessageHandler message)
        {
            Handlers.Add(0, message);
        }

        public static void Clear()
        {
            Handlers.Clear();
        }

        public static void Handle(Session session, object obj)
        {
            if (!Handlers.TryGetValue(0, out var list))
            {
                throw new Exception("消息没有处理");
            }

            foreach (var messageHandler in list)
            {
                messageHandler.Handle(session, obj);
            }
        }
    }
}