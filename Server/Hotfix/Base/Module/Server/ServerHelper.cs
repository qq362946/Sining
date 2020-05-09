using System;
using System.Linq;
using Sining.Config;
using Sining.Message;
using Sining.Tools;

namespace Sining.Module
{
    public static class ServerHelper
    {
        public static STaskCompletionSource STaskCompletionSource;

        private static async STask StartServer(int serverId, string manageServer = null)
        {
            // 根据serverId获取ServerConfig

            var serverConfig = ServerConfigData.Instance.GetConfig(serverId);
            
            // 创建一个服务器Scene

            var serverScene = SceneManagementComponent.Instance.CreateServerScene(serverConfig);

            // 根据serverId获取SceneConfig

            var scenes = SceneConfigData.Instance.GetByServerType(serverConfig.ServerType);

            if (scenes == null) return;

            foreach (var sceneConfig in scenes)
            {
                await SceneManagementComponent.Instance.Create(serverScene, serverConfig, sceneConfig);
            }

            Log.Debug(
                $"Server {ServerTypeConfigData.Instance.GetConfig(serverConfig.ServerType).ServerTypeName} is start...");

            if (!string.IsNullOrWhiteSpace(manageServer))
            {
                if (serverConfig.ServerType == (int) ServerType.ServerManage)
                {
                    return;
                }
               
                new ServerStartFinished {ServerId = serverConfig.Id}.Send(serverScene, manageServer);
            }
        }

        public static STask Start(Options options)
        {
            return options.Single == 1 ? StartServer(options.Server, GetManageServer()) : Start(options.Server);
        }

        public static STask Start(int serverId, bool singleMode = true)
        {
            if (singleMode)
            {
                return StartServer(serverId);
            }

            STaskCompletionSource = new STaskCompletionSource();

            ProcessHelper.Run("dotnet", $"Server.App.dll --Server {serverId} --Single 1", "../Bin");

            return STaskCompletionSource.Task;
        }

        private static string GetManageServer()
        {
            var serverManageConfig = ServerConfigData.Instance.GetServers(ServerType.ServerManage).FirstOrDefault();

            if (serverManageConfig == null)
            {
                throw new Exception("ServerConfig not found ServerType:ServerManage");
            }

            return $"{serverManageConfig.InnerIP}:{serverManageConfig.InnerPort}";
        }
    }
}