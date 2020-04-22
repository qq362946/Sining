using Sining.Config;
using Sining.Event;
using Sining.Message;
using Sining.Tools;

namespace Sining.Module.Server
{
    [ComponentSystem]
    public class StartServerComponentAwakeSystem : AwakeSystem<StartServerComponent, int>
    {
        protected override void Awake(StartServerComponent self, int serverId)
        {
            self.AwakeAsync(serverId).Coroutine();
        }
    }
    
    public static class StartServerComponentSystem
    {
        public static async SVoid AwakeAsync(this StartServerComponent self, int serverId)
        {
            // 单进程模式.
            // 所有服务器都运行在一个服务器里，方便调试时使用。

            if (serverId < 0)
            {
                Log.Info("[单进程模式]开始启动服务器，请稍等...");

                await self.StartUpServerAsync();

                return;
            }

            // 多进程模式.

            if (serverId == 0)
            {
                Log.Info("[多进程模式]开始启动服务器，请稍等...");

                SApp.Scene.AddComponent<NetInnerComponent, string>(self.ManageServer);

                await self.StartUpServerAsync(true);

                return;
            }

            await self.Create(SApp.ServerConfig, true);
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

            ProcessHelper.Run("dotnet", $"Server.App.dll --Server {serverConfig.Id}", "../Bin");
            
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