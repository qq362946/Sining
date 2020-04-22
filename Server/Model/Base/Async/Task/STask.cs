using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sining
{
    [AsyncMethodBuilder(typeof(AsyncSTaskMethodBuilder))]
    public struct STask : ITask
    {
        [DebuggerHidden]
        public static STask CompletedTask => new STask();

        [DebuggerHidden]
        public IAwaiter Awaiter { get;}

        [DebuggerHidden]
        public STask(IAwaiter awaiter)
        {
            Awaiter = awaiter;
        }

        [DebuggerHidden]
        public IAwaiter GetAwaiter() => new STaskAwaiter(this);
    }

    [AsyncMethodBuilder(typeof(AsyncSTaskMethodBuilder<>))]
    public struct STask<T> : ITask<T>
    {
        [DebuggerHidden]
        public static STask CompletedTask => new STask();

        [DebuggerHidden]
        public IAwaiter<T> Awaiter { get;}

        [DebuggerHidden]
        public STask(IAwaiter<T> awaiter)
        {
            Awaiter = awaiter;
        }

        [DebuggerHidden]
        public IAwaiter<T> GetAwaiter() => new STaskAwaiter<T>(this);
    }
}