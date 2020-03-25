using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sining.DataStructure;
using Sining.Event;
using Sining.Module.TimerComponent;
using Sining.Tools;

namespace Sining
{
    [ComponentSystem]
    public class TimerComponentAwakeSystem : AwakeSystem<TimerComponent>
    {
        protected override void Awake(TimerComponent self)
        {
            self.Awake();
        }
    }

    public class TimerComponent : Component
    {
        public static TimerComponent Instance { get; private set; }
        private readonly ConcurrentDictionary<long, ITimer> _timers = new ConcurrentDictionary<long, ITimer>();
        private readonly ConcurrentOneToManyList<long, long> _timeId = new ConcurrentOneToManyList<long, long>(0);
        private readonly ConcurrentQueue<long> _timeOutTime = new ConcurrentQueue<long>();
        private readonly ConcurrentQueue<long> _timeOutTimerIds = new ConcurrentQueue<long>();
        private long _minTime;
        public void Awake()
        {
            Instance = this;
            
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1);
                
                if (_timeId.Count == 0) return;
                
                var timeNow = TimeHelper.Now;

                if (timeNow < _minTime) return;

                foreach (var key in _timeId.Keys)
                {
                    if (key > timeNow)
                    {
                        _minTime = key;
                        break;
                    }

                    _timeOutTime.Enqueue(key);
                }

                while (_timeOutTime.TryDequeue(out var time))
                {
                    foreach(var timerId in _timeId[time])
                    {
                        _timeOutTimerIds.Enqueue(timerId);	
                    }
                    _timeId.RemoveKey(time);
                }

                while (_timeOutTimerIds.TryDequeue(out var timerId))
                {
                    if (!_timers.TryGetValue(timerId, out var timer))
                    {
                        continue;
                    }

                    OneThreadSynchronizationContext.Instance.Post(() => timer.Run(true));
                }

            }, TaskCreationOptions.LongRunning);
        }
        
        public async STask<bool> WaitAsync(long time)
        {
            var tillTime = TimeHelper.Now + time;
            
            if (TimeHelper.Now > tillTime)
            {
                return true;
            }

            var tcs = new STaskCompletionSource<bool>();
            var timer = ComponentFactory.Create<WaitTimer, STaskCompletionSource<bool>>(Scene, tcs, this, true);
            _timers[timer.InstanceId] = timer;
            AddToTimeId(tillTime, timer.InstanceId);

            return await tcs.Task;
        }
        public async STask<bool> WaitTillAsync(long tillTime)
        {
            if (TimeHelper.Now > tillTime)
            {
                return true;
            }

            var tcs = new STaskCompletionSource<bool>();
            var timer = ComponentFactory.Create<WaitTimer, STaskCompletionSource<bool>>(Scene, tcs, this, true);
            _timers[timer.InstanceId] = timer;
            AddToTimeId(tillTime, timer.InstanceId);

            return await tcs.Task;
        }
        public long NewRepeatedTimer(long time, Action action)
        {
            if (time <= 30)
            {
                throw new Exception($"repeated time <= 30");
            }

            var tillTime = TimeHelper.Now + time;
            var timer = ComponentFactory.Create<RepeatedTimer, long, Action>(Scene, time, action, this, true);
            _timers[timer.InstanceId] = timer;
            AddToTimeId(tillTime, timer.InstanceId);
            return timer.InstanceId;
        }
        public RepeatedTimer GetRepeatedTimer(long id)
        {
            if (!_timers.TryGetValue(id, out var timer))
            {
                return null;
            }

            return timer as RepeatedTimer;
        }
        public void Remove(long id)
        {
            if (id == 0 || !_timers.Remove(id, out var timer)) return;

            (timer as IDisposable)?.Dispose();
        }
        public void AddToTimeId(long tillTime, long id)
        {
            _timeId.Add(tillTime, id);
            
            if (tillTime < _minTime)
            {
                _minTime = tillTime;
            }
        }
    }
}