using System;
using CommandLine;
using Sining.Core;
using Sining.Module;
using Sining.Tools;

namespace Sining
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // 服务器会占用8999端口，请不要使用这个端口。
                // 解析命令行
                Options options = null;
                Parser.Default.ParseArguments<Options>(args)
                    .WithNotParsed(error => throw new Exception("Command line format error!"))
                    .WithParsed(option => options = option);
                // 系统初始化
                SiningSystem.Init();
                // 设置服务器ID
                App.Id = options.Server;
                // 逻辑处理组件
                App.Scene.AddComponent<TaskProcessingComponent>();
                // 场景管理组件
                App.Scene.AddComponent<SceneManagementComponent>();
                // 启动服务器
                if (App.Id == 0)
                {
                    Log.Info("开始启动服务器，请稍等...");
                    App.Scene.AddComponent<ProcessWatcherComponent>();
                    App.Scene.AddComponent<NetInnerComponent, string>("127.0.0.1:8999");
                }
                else
                {
                    // 创建服务器
                    ServerFactory.Create();
                }

                // 防止主线程退出
                for (;;) Console.ReadKey();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}