using System;
using System.Diagnostics;

namespace Sining
{
    public struct STaskAwaiter : IAwaiter
    {
        [DebuggerHidden]
        public bool IsCompleted { get;}

        [DebuggerHidden]
        private STask Task { get;}

        [DebuggerHidden]
        public STaskAwaiter(STask sTask)
        {
            Task = sTask;
            IsCompleted = false;
        }

        [DebuggerHidden]
        public void GetResult()
        {
            if (Task.Equals(default) || Task.Awaiter == null) return;

            Task.Awaiter.GetResult();
        }

        [DebuggerHidden]
        public void OnCompleted(Action continuation)
        {
            if (Task.Equals(default) || Task.Awaiter.Equals(default))
            {
                continuation.Invoke();

                return;
            }

            Task.Awaiter.OnCompleted(continuation);
        }

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action continuation)
        {
            if (Task.Equals(default) || Task.Awaiter == null)
            {
                continuation.Invoke();

                return;
            }

            Task.Awaiter.UnsafeOnCompleted(continuation);
        }
    }
    
    public struct STaskAwaiter<T> : IAwaiter<T>
    {
        [DebuggerHidden]
        public bool IsCompleted { get;}

        [DebuggerHidden]
        private STask<T> Task { get;}

        [DebuggerHidden]
        public STaskAwaiter(STask<T> sTask)
        {
            Task = sTask;
            IsCompleted = false;
        }
        
        [DebuggerHidden]
        public T GetResult()
        {
            if (Task.Equals(default) || Task.Awaiter == null)
            {
                return default;
            }

            return Task.Awaiter.GetResult();
        }
        
        [DebuggerHidden]
        public void OnCompleted(Action continuation)
        {
            if (Task.Equals(default) || Task.Awaiter == null)
            {
                continuation.Invoke();
                
                return;
            }
            
            Task.Awaiter.OnCompleted(continuation);
        }

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action continuation)
        {
            if (Task.Equals(default) || Task.Awaiter == null)
            {
                continuation.Invoke();

                return;
            }

            Task.Awaiter.UnsafeOnCompleted(continuation);
        }
    }
}