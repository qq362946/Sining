using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sining.Event;
using Sining.Tools;

namespace Sining.Network.Actor
{
    public class ActorMessageComponent : Component
    {
        public static ActorMessageComponent Instance;
        public const int TimeOut = 30 * 1000;
        public int RpcId;
        public Task Task;
        public readonly Dictionary<int, ActorTaskComponent> RequestCallback = new Dictionary<int, ActorTaskComponent>();
        public readonly List<int> TimeoutActors = new List<int>();
        public override void Dispose()
        {
            if(IsDispose) return;
            
            Task.Dispose();
            
            foreach (var requestCallbackValue in RequestCallback.Values)
            {
                requestCallbackValue.Dispose();
            }
            
            RequestCallback.Clear();
            Instance = null;
            
            base.Dispose();
        }
    }
}