using System;
using Sining.Event;
using Sining.Module;

namespace Sining.Network.Actor
{
    public enum ActorMailBoxType
    {
        None = 0,
        Handler = 1,
        Forward = 2
    }
    
    public interface IActorMailBox
    { 
        void Handle(Session session, IActorMessage message);
    }

    [ComponentSystem]
    public class ActorMailBoxComponentAwakeSystem : AwakeSystem<ActorMailBoxComponent, ActorMailBoxType>
    {
        protected override void Awake(ActorMailBoxComponent self, ActorMailBoxType actorMailBoxType)
        {
            self.Awake(actorMailBoxType);
        }
    }

    public class ActorMailBoxComponent : Component
    {
        public IActorMailBox ActorMailBox { get; private set; }

        public void Awake(ActorMailBoxType actorMailBoxType)
        {
            ActorMailBox = actorMailBoxType switch
            {
                ActorMailBoxType.Handler => (IActorMailBox) AddComponent<ActorHandlerMailBoxComponent>(),
                ActorMailBoxType.Forward => AddComponent<ActorForwardMailBoxComponent>(),
                _ => throw new Exception($"not found is {actorMailBoxType.ToString()}")
            };
        }
    }

    /// <summary>
    /// 只处理消息
    /// </summary>
    public class ActorHandlerMailBoxComponent : Component, IActorMailBox
    {
        public void Handle(Session session, IActorMessage message)
        {
            ActorDispatcherComponent.Instance.Handle(session, Parent, message);
        }
    }
    
    /// <summary>
    /// 不处理消息，只转发消息
    /// </summary>
    public class ActorForwardMailBoxComponent : Component, IActorMailBox
    {
        public void Handle(Session session, IActorMessage message)
        {
            message.ActorId = 0;
            session.Send(message);
        }
    }
}