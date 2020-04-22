using System;
using System.Collections.Generic;
using System.Linq;
using Sining.DataStructure;
using Sining.Module;
using Sining.Tools;

namespace Sining.Network.Actor
{
    public class ActorDispatcherComponent : Component
    {
        public static ActorDispatcherComponent Instance;
        private readonly OneToManyList<Type, IActorMessageHandler> _actorHandlers = new OneToManyList<Type, IActorMessageHandler>();
        public void Init()
        {
            Instance = this;
            
            foreach (var allTypes in AssemblyManagement.AllType.Values)
            {
                foreach (var type in allTypes.Where(d =>
                    d.IsDefined(typeof(ActorMessageSystemAttribute), true)))
                {
                    var obj = Activator.CreateInstance(type);
                
                    if (!(obj is IActorMessageHandler actorMessageHandler))
                    {
                        throw new Exception($"message handle {type.Name} 需要继承 IActorMessageHandler");
                    }
                
                    AddHandler(actorMessageHandler.Type(), actorMessageHandler);
                }
            }
        }
        public async STask Handle(Session session, Component component, object obj)
        {
            var list = GetHandler(obj.GetType());

            if (list == null || list.Count == 0)
            {
                Log.Warning("The message was not processed");
                return;
            }

            foreach (var messageHandler in list)
            {
               await messageHandler.Handle(session, component, obj);
            }
        }
        public List<IActorMessageHandler> GetHandler(Type type)
        {
            _actorHandlers.TryGetValue(type, out var list);

            return list;
        }
        public void AddHandler(Type type, IActorMessageHandler message)
        {
            _actorHandlers.Add(type, message);
        }
        public void ReLoad()
        {
            Clear();
            Init();
        }
        private void Clear()
        {
            _actorHandlers.Clear();
        }
        public override void Dispose()
        {
            if (IsDispose) return;

            Clear();
            Instance = null;
            
            base.Dispose();
        }
    }
}