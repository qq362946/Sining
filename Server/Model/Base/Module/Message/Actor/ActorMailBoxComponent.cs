using System;
using Sining.Event;
using Sining.Module;

namespace Sining.Network.Actor
{
    [ComponentSystem]
    public class ActorMailBoxComponentAwakeSystem : AwakeSystem<ActorMailBoxComponent>
    {
        protected override void Awake(ActorMailBoxComponent self)
        {
            self.Awake();
        }
    }

    public class ActorMailBoxComponent : Component
    {
        public void Awake()
        {
            
        }
    }
}