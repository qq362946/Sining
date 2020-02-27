using System;
using System.Threading;
using Sining.Event;
using Sining.Module;
using Sining.Network;

namespace Sining.Core
{
    [MessageSystem]
    public class OnGetMessage : MessageHandler<string>
    {
        protected override void Run(Session session, string message)
        {
            Console.WriteLine($"3333333     {message}");
            Console.WriteLine($"收到一个消息 ThreadId:{Thread.CurrentThread.ManagedThreadId}");
        }
    }
}