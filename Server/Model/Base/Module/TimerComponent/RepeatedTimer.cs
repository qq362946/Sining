using System;
using Sining.Event;
using Sining.Tools;

namespace Sining.Module.TimerComponent
{
    [ComponentSystem]
    public class RepeatedTimerAwakeSystem : AwakeSystem<RepeatedTimer, long, Action<bool>>
    {
        protected override void Awake(RepeatedTimer self, long repeatedTime, Action<bool> callback)
        {
            self.Awake(repeatedTime, callback);
        }
    }
    
    public class RepeatedTimer : Component, ITimer
    {
        private long _startTime;
        private long _repeatedTime;
        private int _count;
        private Action<bool> _callback;
        
        public void Awake(long repeatedTime, Action<bool> callback)
        {
            _startTime = TimeHelper.Now;
            _repeatedTime = repeatedTime;
            _callback = callback;
            _count = 1;
        }
        public void Run(bool isTimeout)
        {
            ++_count;
            var timerComponent = GetParent<Sining.TimerComponent>();
            var tillTime = _startTime + _repeatedTime * _count;
            timerComponent.AddToTimeId(tillTime, InstanceId);

            try
            {
                _callback.Invoke(isTimeout);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            var instanceId = InstanceId;

            if (instanceId == 0)
            {
                Log.Error($"RepeatedTimer可能多次释放了");
                return;
            }

            base.Dispose();

            _startTime = 0;
            _repeatedTime = 0;
            _callback = null;
            _count = 0;
        }
    }
}