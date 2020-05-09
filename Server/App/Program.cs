using System;
using System.Threading;
using CommandLine;
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
                // 将其他线程的数据同步上线文。保证都在同一个线程里执行。
                SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);
                // 解析命令行
                Options options = null;
                Parser.Default.ParseArguments<Options>(args)
                    .WithNotParsed(error => throw new Exception("Command line format error!"))
                    .WithParsed(option => options = option);
                // 系统初始化
                SiningSystem.Init();
                // 设置服务器ID
                MainScene.Id = options.Server;
                // 启动服务器组件
                MainScene.Scene.AddComponent<StartServerComponent, Options>(options);
                // 防止主线程退出
                for (;;)
                {
                    try
                    {
                        Thread.Sleep(1);
                        OneThreadSynchronizationContext.Instance.Update();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}