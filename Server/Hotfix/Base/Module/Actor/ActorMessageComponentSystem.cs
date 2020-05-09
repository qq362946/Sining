using System;
using System.Threading;
using System.Threading.Tasks;
using Sining.Event;
using Sining.Network;
using Sining.Network.Actor;
using Sining.Tools;

namespace Sining.Module.Actor
{
    [ComponentSystem]
    public class ActorMessageComponentAwakeSystem : AwakeSystem<ActorMessageComponent>
    {
        protected override void Awake(ActorMessageComponent self)
        {
            self.Awake();
        }
    }
    
    public static class ActorMessageComponentSystem
    {
        public static void Awake(this ActorMessageComponent self)
        {
            self.Task = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(ActorMessageComponent.TimeOut);

                var timeNow = TimeHelper.Now;

                foreach (var (key, value) in self.RequestCallback)
                {
                    if (timeNow < value.SendTime + ActorMessageComponent.TimeOut)
                    {
                        continue;
                    }

                    self.TimeoutActors.Add(key);
                }

                foreach (var timeoutActor in self.TimeoutActors)
                {
                    if (!self.RequestCallback.Remove(timeoutActor, out var actorTask))
                    {
                        continue;
                    }

                    actorTask.Callback.Invoke(new ActorResponse() {ErrorCode = ModelErrorCode.ErrActorTimeout});
                }

                self.TimeoutActors.Clear();

            }, TaskCreationOptions.LongRunning);

            ActorMessageComponent.Instance = self;
        }
        
        public static void Send(this ActorMessageComponent self, IActorMessage actorMessage)
        {
            if (actorMessage.ActorId == 0)
            {
                throw new Exception($"ActorId is 0: {actorMessage.ToBytes()}");
            }

            var appId = actorMessage.ActorId.GetAppId();

            actorMessage.Send(self.Scene, (int) appId);
        }

        public static STask<TResponse> Call<TResponse>(this ActorMessageComponent self, IActorRequest request)
            where TResponse : IActorResponse
        {
            var rpcId = ++self.RpcId;

            var tcs = new STaskCompletionSource<TResponse>();

            self.RequestCallback[rpcId] = ComponentFactory.Create<ActorTaskComponent, Action<IActorResponse>>
            (self.Scene, response =>
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

            self.Send(request);

            return tcs.Task;
        }

        public static void Receive(this ActorMessageComponent self, IActorResponse response)
        {
            if (!self.RequestCallback.TryGetValue(response.RpcId, out var actorTaskComponent))
            {
                Log.Error($"not found ActorRpc, maybe request timeout, response message: {response.ToJson()}");
                return;
            }

            self.RequestCallback.Remove(response.RpcId);

            actorTaskComponent.Callback(response);
        }
    }
}