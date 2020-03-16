using System.Threading;
using Sining;
using Sining.Message;
using Sining.Module;
using Sining.Network;

namespace Server.Hotfix
{
    [MessageSystem]
    public class TestMessageApiHandler : MessageHandler<TestMessage>
    {
        protected override async STask Run(Session session, TestMessage message)
        {
            Log.Debug($"接收到一个消息：" +
                      $"Name:{message.Name} " +
                      $"Number:{message.Number} " +
                      $"Page:{message.Page} " +
                      $"ThreadId:{Thread.CurrentThread.ManagedThreadId} " +
                      $"Server:{SApp.Id}");

            await STask.CompletedTask;
        }
    }
    
    [MessageSystem]
    public class LoginRequestHandler : MessageHandler<LoginRequest>
    {
        protected override async STask Run(Session session, LoginRequest message)
        {
            Log.Debug(
                $"接收到一个消息：UserName:{message.UserName} PassWord:{message.PassWord} ThreadId:{Thread.CurrentThread.ManagedThreadId}");

            await STask.CompletedTask;
        }
    }

    [MessageSystem]
    public class GetNameRequestHandler : RPCMessageHandler<GetNameRequest, GetNameResponse>
    {
        protected override async STask Run(Session session, GetNameRequest request, GetNameResponse response)
        {
            response.Name = request.Name + "1233444";

            Log.Debug($"接收到一个消息：GetNameRequest:{request.Name} ThreadId:{Thread.CurrentThread.ManagedThreadId}");

            await STask.CompletedTask;
        }
    }
}