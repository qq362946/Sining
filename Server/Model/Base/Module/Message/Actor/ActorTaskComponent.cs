using System;
using Sining.Event;
using Sining.Tools;

namespace Sining.Network.Actor
{
    [ComponentSystem]
    public class ActorTaskComponentAwakeSystem : AwakeSystem<ActorTaskComponent, Action<IActorResponse>>
    {
        protected override void Awake(ActorTaskComponent self, Action<IActorResponse> callback)
        {
            self.Awake(callback);
        }
    }

    public class ActorTaskComponent : Component
    {
        public long SendTime { get; private set; }
        public Action<IActorResponse> Callback { get; private set; }

        public void Awake(Action<IActorResponse> callback)
        {
            SendTime = TimeHelper.Now;
            Callback = callback;
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            SendTime = 0;
            Callback = null;

            base.Dispose();
        }
    }
}