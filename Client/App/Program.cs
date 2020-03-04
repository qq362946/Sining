using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using Sining;
using Sining.Core;
using Sining.Message;
using Sining.Tools;
using Sining.Module;

namespace Client.App
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // 系统初始化
            SiningSystem.Init();
            // 设置AppID
            SApp.Id = 0;
            // 逻辑处理组件
            SApp.Scene.AddComponent<TaskProcessingComponent>();
            // 挂载网络组件（可以同时挂载多个，也就是多个网络连接）
            SApp.Scene.AddComponent<NetOuterComponent, NetworkProtocolType>(NetworkProtocolType.TCP);

            // TCP
            // 获取网络组件
            var tcpNetOuterComponent = SApp.Scene.GetComponent<NetOuterComponent>();
            // 根据地址连接到服务器
            var tcpSession = tcpNetOuterComponent.Create("127.0.0.1:10000");
            Run(tcpSession).Coroutine();
            
            // WebSocket
            var webSocketNetOuterComponent =
                ComponentFactory.CreateOnly<NetOuterComponent, NetworkProtocolType>(NetworkProtocolType.WebSocket);
            var webSocketSession = webSocketNetOuterComponent.Create("ws://127.0.0.1:8889/");
            Run(webSocketSession).Coroutine();

            // HTTP
            var httpNetOuterComponent =
                ComponentFactory.CreateOnly<NetOuterComponent, NetworkProtocolType>(NetworkProtocolType.HTTP);
            var httpSession = httpNetOuterComponent.Create("http://127.0.0.1:8888/");
            Run(httpSession).Coroutine();
            
            
            for (;;)
            {
                Console.ReadKey();
                httpSession.Send(new TestMessage()
                {
                    Name = "大宁", Number = 123
                });
            }
        }

        private static async SVoid Run(Session session)
        {
            var result = await session.Call<GetNameResponse>(new GetNameRequest()
            {
                Name = "宁2"
            });
            
            Console.WriteLine($"result:{result.ToJson()}");
        }
    }
}