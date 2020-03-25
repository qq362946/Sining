using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Sining
{
    public class OneThreadSynchronizationContext : SynchronizationContext
    {
        public static OneThreadSynchronizationContext Instance { get; } = new OneThreadSynchronizationContext();
        private readonly int _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();
        private Action _action;
        public void Update()
        {
            while (true)
            {
                if (!_queue.TryDequeue(out _action)) return;
               
                _action();
            }
        }

        public void Post(Action action)
        {
            if (Thread.CurrentThread.ManagedThreadId == _mainThreadId)
            {
                action();
            }

            _queue.Enqueue(action);
        }
        public override void Post(SendOrPostCallback callback, object state)
        {
            if (Thread.CurrentThread.ManagedThreadId == _mainThreadId)
            {
                callback(state);
                return;
            }
			
            _queue.Enqueue(() => { callback(state); });
        }
    }
}