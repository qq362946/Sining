using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace Sining
{
    public class AsyncSTaskMethodBuilder
    {
        private readonly STaskCompletionSource _sTaskCompletionSource= new STaskCompletionSource();

        public STask Task => _sTaskCompletionSource.Task;

        [DebuggerHidden]
        public static AsyncSTaskMethodBuilder Create() => new AsyncSTaskMethodBuilder();

        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine){}

        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            _sTaskCompletionSource.SetException(exception);
        }
        
        [DebuggerHidden]
        public void SetResult() 
        {
            _sTaskCompletionSource.SetResult();
        }

        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
        }
    }

    public class AsyncSTaskMethodBuilder<T>
    {
        private readonly STaskCompletionSource<T> _sTaskCompletionSource= new STaskCompletionSource<T>();

        public STask<T> Task => _sTaskCompletionSource.Task;

        [DebuggerHidden]
        public static AsyncSTaskMethodBuilder<T> Create() => new AsyncSTaskMethodBuilder<T>();

        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine){}

        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            _sTaskCompletionSource.SetException(exception);
        }
        
        [DebuggerHidden]
        public void SetResult(T result) 
        {
            _sTaskCompletionSource.SetResult(result);
        }
        
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }
        
        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
        }
    }
}