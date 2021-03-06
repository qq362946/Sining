// using System;
// using System.Threading;
// using Sining;
// using Sining.Message;
// using Sining.Model;
// using Sining.Module;
// using Sining.Network;
// using Sining.Network.Actor;
// using Sining.Tools;
//
// namespace Server.Hotfix
// {
//     [HTTPApiController]
//     public class TestHttp : HTTPControllerBase
//     {
//         [Post("/api/login1")]
//         public ActionResult test1(string name, string test1)
//         {  
//             return Success();
//         }
//         [PostJson("/api/login")]
//         public ActionResult Test(TestPostModel testPostModel)
//         {
//             Log.Debug(testPostModel);
//             Log.Debug($"接收到Post请求:mame:{testPostModel.Name}passWord{testPostModel.PassWord}");
//            // await testPostModel.DataBase().Insert(testPostModel);
//             return Success("111");
//         }
//
//         [GetJson(("/api/loginget"))]
//         public ActionResult DTest(TestPostModel testPostMode)
//         {
//             Log.Debug($"接收Get请求:mame:{testPostMode.Name}passWord{testPostMode.PassWord}");
//
//             return Success("1132424324234");
//         }
//     }
//     
//     [MessageSystem]
//     public class TestMessageApiHandler : MessageHandler<TestMessage>
//     {
//         protected override async STask Run(Session session, TestMessage message)
//         {
//             Log.Debug($"接收到一个消息：" +
//                       $"Name:{message.Name} " +
//                       $"Number:{message.Number} " +
//                       $"Page:{message.Page} " +
//                       $"ThreadId:{Thread.CurrentThread.ManagedThreadId} " +
//                       $"Server:{SApp.Id}");
//
//             await STask.CompletedTask;
//         }
//     }
//     
//     [MessageSystem]
//     public class LoginRequestHandler : MessageHandler<LoginRequest>
//     {
//         protected override async STask Run(Session session, LoginRequest message)
//         {
//             Log.Debug(
//                 $"接收到一个消息：UserName:{message.UserName} PassWord:{message.PassWord} ThreadId:{Thread.CurrentThread.ManagedThreadId}");
//
//             await STask.CompletedTask;
//         }
//     }
//
//     [MessageSystem]
//     public class GetNameRequestHandler : RPCMessageHandler<GetNameRequest, GetNameResponse>
//     {
//         protected override async STask Run(Session session, GetNameRequest request, GetNameResponse response,
//             Action reply)
//         {
//             response.Name = request.Name + "1233444";
//
//             Log.Debug($"接收到一个消息：GetNameRequest:{request.Name} ThreadId:{Thread.CurrentThread.ManagedThreadId}");
//
//             await STask.CompletedTask;
//         }
//     }
//
//     [ActorMessageSystem]
//     public class TestActorMessageHandler : ActorMessageMessageHandler<TestActrModelComponent, TestActorMessage>
//     {
//         protected override async STask Run(TestActrModelComponent testActrModelComponent, TestActorMessage message)
//         {
//             Log.Debug($"收到一个Actor消息:{message.ToJson()} TestActrModelComponent:{testActrModelComponent.ToJson()}");
//
//             await STask.CompletedTask;
//         }
//     }
//     
//     [ActorMessageSystem]
//     public class TestActorCallMessageHandler : ActorRPCMessageHandler<TestActrModelComponent,TestActorCallRequestMessage,TestActorCallResponseMessage>
//     {
//         protected override async STask Run(TestActrModelComponent testActrModelComponent,
//             TestActorCallRequestMessage actorRequest,
//             TestActorCallResponseMessage actorResponse, Action reply)
//         {
//             Log.Debug(
//                 $"收到一个ActorCall消息:{actorRequest.ToJson()} TestActrModelComponent:{testActrModelComponent.ToJson()}");
//
//             actorResponse.Message = actorRequest.Name;
//
//             await STask.CompletedTask;
//         }
//     }
// }