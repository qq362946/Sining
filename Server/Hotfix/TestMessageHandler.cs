using System.Threading;
using Sining;
using Sining.Event;
using Sining.Message;
using Sining.Module;
using Sining.Network;

namespace Server.Hotfix
{
    [MessageSystem]
    public class TestMessageHandler : MessageHandler<TestMessage>
    {
        protected override void Run(Session session, TestMessage message)
        {
            Log.Debug($"接收到一个消息：" +
                      $"Name:{message.Name} " +
                      $"Number:{message.Number} " +
                      $"Page:{message.Page} " +
                      $"ThreadId:{Thread.CurrentThread.ManagedThreadId} " +
                      $"Server:{SApp.Id}");
        }
    }
    
    [MessageSystem]
    public class LoginRequestHandler : MessageHandler<LoginRequest>
    {
        protected override void Run(Session session, LoginRequest message)
        {
            Log.Debug($"接收到一个消息：UserName:{message.UserName} PassWord:{message.PassWord} ThreadId:{Thread.CurrentThread.ManagedThreadId}");
        }
    }

    [MessageSystem]
    public class GetNameRequestHandler : RPCMessageHandler<GetNameRequest, GetNameResponse>
    {
        protected override void Run(Session session, GetNameRequest request, GetNameResponse response)
        {
            response.Name = request.Name + "1233444";

            Log.Debug($"接收到一个消息：GetNameRequest:{request.Name} ThreadId:{Thread.CurrentThread.ManagedThreadId}");
        }
    }
}