using Sining.Event;

namespace Sining.Module.TimerComponent
{
    [ComponentSystem]
    public class WaitTimerAwakeSystem : AwakeSystem<WaitTimer,STaskCompletionSource<bool>>
    {
        protected override void Awake(WaitTimer self, STaskCompletionSource<bool> tcs)
        {
            self.Awake(tcs);
        }
    }

    public class WaitTimer : Component, ITimer
    {
        private STaskCompletionSource<bool> _tcs;

        public void Awake(STaskCompletionSource<bool> tcs)
        {
            _tcs = tcs;
        }
        
        public void Run(bool isTimeout)
        {
            var tcs = _tcs;
            GetParent<Sining.TimerComponent>().Remove(InstanceId);
            tcs.SetResult(isTimeout);
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            _tcs = null;
            
            base.Dispose();
        }
    }
}