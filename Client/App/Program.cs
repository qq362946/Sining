﻿using System;
using MongoDB.Bson;
using Sining;
using Sining.Message;
using Sining.Tools;
using Sining.Module;
using Sining.Network;

namespace Client.App
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // 系统初始化
                SiningSystem.Init();

                // 挂载TCP网络组件（可以同时挂载多个，也就是多个网络连接）
                SApp.Scene.AddComponent<NetOuterComponent, MessagePacker, NetworkProtocolType>(
                    ComponentFactory.Create<ProtobufMessagePacker>(),
                    NetworkProtocolType.TCP);

                // 获取网络组件
                var tcpNetOuterComponent = SApp.Scene.GetComponent<NetOuterComponent>();

                // 根据地址连接到服务器
                var tcpSession = tcpNetOuterComponent.Create("127.0.0.1:10000");
                TextCall(tcpSession).Coroutine();

                // WebSocket
                var webSocketNetOuterComponent =
                    ComponentFactory.CreateOnly<NetOuterComponent, MessagePacker, NetworkProtocolType>(
                        ComponentFactory.Create<ProtobufMessagePacker>(),
                        NetworkProtocolType.WebSocket);

                var webSocketSession = webSocketNetOuterComponent.Create("ws://127.0.0.1:8889/");
                TextCall(webSocketSession).Coroutine();

                // HTTP

                SApp.Scene.AddComponent<HttpClientComponent>();

                HttpTestCall().Coroutine();

                for (;;) Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        private static async SVoid HttpTestCall()
        {
            HttpClientComponent.Instance.Send(new TestMessage()
            {
                Name = "张思", Number = 666, Page = 9
            }, "http://127.0.0.1:8888");
            
            var result = await HttpClientComponent.Instance.Call<GetNameResponse>(new GetNameRequest()
            {
                Name = "宁2"
            },"http://127.0.0.1:8888");
            
            Console.WriteLine($"result:{result.ToJson()}");
        }

        private static async SVoid TextCall(Session session)
        {
            var result = await session.Call<GetNameResponse>(new GetNameRequest()
            {
                Name = "宁2"
            });
            
            Console.WriteLine($"result:{result.ToJson()}");
        }
    }
}