using System;

namespace Sining
{
    public class SVoidAwaiter : IAwaiter
    {
        public bool IsCompleted => true;
        
        public void GetResult()
        {
            throw new InvalidOperationException("ETAvoid can not await, use Coroutine method instead!");
        }

        public void OnCompleted(Action continuation) { }

        public void UnsafeOnCompleted(Action continuation) { }
    }
}