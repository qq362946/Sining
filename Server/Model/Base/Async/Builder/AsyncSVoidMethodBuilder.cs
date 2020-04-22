using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using Sining;

namespace Sining
{
    public class AsyncSVoidMethodBuilder
    {
        public SVoid Task => default;

        [DebuggerHidden]
        public static AsyncSVoidMethodBuilder Create() => new AsyncSVoidMethodBuilder();

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
            Log.Error(exception);
        }
        
        [DebuggerHidden]
        public void SetResult() { }

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