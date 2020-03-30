using System;
using System.Diagnostics;
using System.Threading;
using CommandLine;
using MongoDB.Bson;
using Server.Network;
using Sining.Message;
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
                // 服务器会占用8999端口，请不要使用这个端口。
                // 解析命令行
                Options options = null;
                Parser.Default.ParseArguments<Options>(args)
                    .WithNotParsed(error => throw new Exception("Command line format error!"))
                    .WithParsed(option => options = option);
                // 系统初始化
                SiningSystem.Init();
                // 设置服务器ID
                SApp.Id = options.Server < 0 ? 0 : options.Server;
                // 启动服务器组件
                SApp.Scene.AddComponent<StartServerComponent, int>(options.Server);
                // 初始化数据库(Mongo数据库不需要)
                // 这个方法会帮助创建数据库表，需要在SqlDBComponent.Init方法里添加需要创建的表类型
                // DBHelper.Init();
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