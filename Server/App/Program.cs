using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sining.Core;
using Sining.Event;
using Sining.Message;
using Sining.Module;

namespace Sining
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                App.Id = 1;
                // 事件初始化
                EventManagement.Initialization();
                // 任务消息处理组件
                App.Scene.AddComponent<TaskProcessingComponent>();
                // 服务器外网服务组件
                var netOuterComponent = App.Scene.AddComponent<NetOuterComponent, string>("http://127.0.0.1:8888/");
                // 测试外网消息包
                // var session = App.Scene.AddComponent<Scene>().AddComponent<NetOuterComponent>()
                //     .Create("ws://127.0.0.1:8888/");
                //var netOuterComponent = App.Scene.AddComponent<NetOuterComponent, string>("http://127.0.0.1:8888/");

                Log.Info("服务器运行中...");

                for (;;) Console.ReadKey();
            }
            catch (Exception e)
            {
                Log.Fatal(e);
            }
        }
    }
}