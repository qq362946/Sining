using System;
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