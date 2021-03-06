using System;
using System.Collections.Generic;
using System.Linq;
using Sining.DataStructure;
using Sining.Module;
using Sining.Tools;
using System.Reflection;

namespace Sining.Network
{
    public class MessageDispatcherManagement : Component
    {
        public static MessageDispatcherManagement Instance;
        private readonly OneToManyList<Type, IMessageHandler> _handlers =
            new OneToManyList<Type, IMessageHandler>(0);
        public void Init()
        {
            foreach (var allTypes in AssemblyManagement.AllType.Values)
            {
                foreach (var type in allTypes.Where(d =>
                    d.IsDefined(typeof(MessageSystemAttribute), true)))
                {
                    var obj = Activator.CreateInstance(type);

                    if (!(obj is IMessageHandler messageHandler))
                    {
                        throw new Exception($"message handle {type.Name} 需要继承 IMessageHandler");
                    }

                    AddHandler(messageHandler.Type(), messageHandler);
                }
            }

            Instance = this;
        }
        public List<IMessageHandler> GetHandler(Type type)
        {
            _handlers.TryGetValue(type, out var list);

            return list;
        }
        public void AddHandler(Type type, IMessageHandler message)
        {
            _handlers.Add(type, message);
        }
        public async STask Handle(Session session, object obj)
        {
            if (!_handlers.TryGetValue(obj.GetType(), out var list))
            {
                Log.Warning("The message was not processed");
                return;
            }

            foreach (var messageHandler in list)
            {
               await messageHandler.Handle(session, obj);
            }
        }
        public void ReLoad()
        {
            Clear();
            Init();
        }
        private void Clear()
        {
            _handlers.Clear();
        }
        public override void Dispose()
        {
            if (IsDispose) return;

            Clear();
            
            base.Dispose();
        }
    }
}