using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Sining;
using Sining.Event;

namespace Sining.Core
{
    [ComponentSystem]
    public class TaskSchedulingComponentAwakeSystem : AwakeSystem<TaskProcessingComponent>
    {
        protected override void Awake(TaskProcessingComponent self)
        {
            self.Awake();
        }
    }

    public class TaskProcessingComponent : Component
    {
        public static TaskProcessingComponent Instance { get; private set; }
        
        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();

        private Thread _taskThread;

        public void Add(Action action)
        {
            _queue.Enqueue(action);
        }

        public void Awake()
        {
            Instance = this;
            try
            {
                _taskThread = new Thread(() =>
                {
                    try
                    {
                        for (;;)
                        {
                            try
                            {
                                if (!_queue.TryDequeue(out var action))
                                {
                                    Thread.Sleep(1);
                                    continue;
                                }

                                action();
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                })
                {
                    IsBackground = true
                };

                _taskThread.Start();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public override void Dispose()
        {
            if (IsDispose) return;
            
            _taskThread.Abort();
            _taskThread.Join();
            _taskThread = null;
            _queue.Clear();
            Instance = null;
            
            base.Dispose();
        }
    }
}