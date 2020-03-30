using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sining.Event;
using Sining.Tools;

namespace Sining.Network.Actor
{
    [ComponentSystem]
    public class ActorMessageComponentAwakeSystem : AwakeSystem<ActorMessageComponent>
    {
        protected override void Awake(ActorMessageComponent self)
        {
            self.Awake();
        }
    }

    public class ActorMessageComponent : Component
    {
        private const int TimeOut = 30 * 1000;
        private int _rpcId;
        private Task _task;
        private readonly Dictionary<int, ActorTaskComponent> _requestCallback = new Dictionary<int, ActorTaskComponent>();
        private readonly List<int> _timeoutActors = new List<int>();

        public void Awake()
        {
            _task = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(TimeOut);

                var timeNow = TimeHelper.Now;

                foreach ((int key, ActorTaskComponent value) in _requestCallback)
                {
                    if (timeNow < value.SendTime + TimeOut)
                    {
                        continue;
                    }

                    _timeoutActors.Add(key);
                }

                foreach (var timeoutActor in _timeoutActors)
                {
                    if (!_requestCallback.Remove(timeoutActor, out var actorTask))
                    {
                        continue;
                    }

                    actorTask.Callback.Invoke(new ActorResponse() {ErrorCode = ErrorCode.ErrActorTimeout});
                }

                _timeoutActors.Clear();

            }, TaskCreationOptions.LongRunning);
        }

        public void Send(IActorMessage actorMessage)
        {
            if (actorMessage.ActorId == 0)
            {
                throw new Exception($"ActorId is 0: {actorMessage.ToBytes()}");
            }

            var appId = actorMessage.ActorId.GetAppId();

            actorMessage.Send((int) appId);
        }

        public STask<TResponse> Call<TResponse>(IActorRequest request) where TResponse : IActorResponse
        {
            var rpcId = ++_rpcId;

            var tcs = new STaskCompletionSource<TResponse>();

            _requestCallback[rpcId] = ComponentFactory.Create<ActorTaskComponent, Action<IActorResponse>>
            (Scene, response =>
                {
                    if (response.ErrorCode > 0)
                    {
                        tcs.SetException(new Exception($"Rpc error errorCode: {response.ErrorCode}"));
                        return;
                    }

                    tcs.SetResult((TResponse) response);
                }
            );

            request.RpcId = rpcId;

            Send(request);

            return tcs.Task;
        }

        public override void Dispose()
        {
            if(IsDispose) return;
            
            _task.Dispose();
            
            foreach (var requestCallbackValue in _requestCallback.Values)
            {
                requestCallbackValue.Dispose();
            }
            
            _requestCallback.Clear();
            
            base.Dispose();
        }
    }
}