using System.Runtime.CompilerServices;

namespace Sining
{
    [AsyncMethodBuilder(typeof(AsyncSVoidMethodBuilder))]
    public struct SVoid : ITask
    {
        public IAwaiter GetAwaiter() => new SVoidAwaiter();
        
        /// <summary>
        /// 此方法没有任何用处，只是消除编译器的警告。
        /// </summary>
        public void Coroutine(){}
    }
}