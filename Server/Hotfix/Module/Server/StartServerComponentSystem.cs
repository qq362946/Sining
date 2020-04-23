using System;
using Sining.Config;
using Sining.Event;
using Sining.Message;
using Sining.Tools;

namespace Sining.Module.Server
{
    [ComponentSystem]
    public class StartServerComponentAwakeSystem : AwakeSystem<StartServerComponent, Options>
    {
        protected override void Awake(StartServerComponent self, Options options)
        {
            self.AwakeAsync(options).Coroutine();
        }
    }
    
    public static class StartServerComponentSystem
    {
        public static async SVoid AwakeAsync(this StartServerComponent self, Options options)
        {
            switch (options.Server)
            {
                case -1:
                    Log.Info("[单进程模式]开始启动服务器，请稍等...");
                    await self.StartUpServerAsync();
                    return;
                case 0:
                    Log.Info("[多进程模式]开始启动服务器，请稍等...");
                    SApp.Scene.AddComponent<NetInnerComponent, string>(self.ManageServer);
                    await self.StartUpServerAsync(true);
                    return;
                default:
                    await self.Create(SApp.ServerConfig, Convert.ToBoolean(options.Single));
                    break;
            }
        }

        private static async STask StartUpServerAsync(this StartServerComponent self, bool runProcess = false)
        {
            foreach (var serverConfig in ServerConfigData.Instance.GetAllConfig())
            {
                if (!runProcess)
                {
                    await self.Create(serverConfig);

                    continue;
                }

                await self.RunServer(serverConfig);
            }
        }

        private static STask RunServer(this StartServerComponent self, ServerConfig serverConfig)
        {
            StartServerComponent.STaskCompletionSource = new STaskCompletionSource();

            ProcessHelper.Run("dotnet", $"Server.App.dll --Server {serverConfig.Id} --Single 1", "../Bin");
            
            return StartServerComponent.STaskCompletionSource.Task;
        }

        private static async STask Create(this StartServerComponent self, ServerConfig serverConfig,
            bool runProcess = false)
        {
            if (!string.IsNullOrWhiteSpace(serverConfig.InnerIP) &&
                serverConfig.InnerPort > 0)
            {
                ComponentFactory.Create<NetInnerComponent, string>(
                    SApp.Scene, $"{serverConfig.InnerIP}:{serverConfig.InnerPort}", SApp.Scene, true);
            }

            var scenes = SceneConfigData.Instance.GetByServer(serverConfig.Id);

            if (scenes == null) return;

            foreach (var sceneConfig in scenes)
            {
                await SceneManagementComponent.Instance.Create(serverConfig, sceneConfig);
            }

            if (runProcess)
            {
                new ServerStartFinished {ServerId = serverConfig.Id}.Send(self.ManageServer);
            }

            Log.Debug($"Server:{serverConfig.ServerType} 启动完成!");
        }
    }
}