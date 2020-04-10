using Sining.Config;
using Sining.Event;
using Sining.Message;
using Sining.Tools;

namespace Sining.Module
{
    [ComponentSystem]
    public class StartServerComponentAwakeSystem : AwakeSystem<StartServerComponent, int>
    {
        protected override void Awake(StartServerComponent self, int serverId)
        {
            self.Awake(serverId);
        }
    }

    public class StartServerComponent : Component
    {
        public static STaskCompletionSource STaskCompletionSource;
        private const string ManageServer = "127.0.0.1:8999";

        public void Awake(int serverId)
        {
            AwakeAsync(serverId).Coroutine();
        }

        private async SVoid AwakeAsync(int serverId)
        {
            // 单进程模式.
            // 所有服务器都运行在一个服务器里，方便调试时使用。

            if (serverId < 0)
            {
                Log.Info("[单进程模式]开始启动服务器，请稍等...");

                await StartUpServerAsync();

                return;
            }

            // 多进程模式.

            if (serverId == 0)
            {
                Log.Info("[多进程模式]开始启动服务器，请稍等...");

                SApp.Scene.AddComponent<NetInnerComponent, string>(ManageServer);

                await StartUpServerAsync(true);

                return;
            }

            await Create(SApp.ServerConfig, true);
        }

        private async STask StartUpServerAsync(bool runProcess = false)
        {
            foreach (var serverConfig in ServerConfigData.Instance.GetAllConfig())
            {
                if (!runProcess)
                {
                    await Create(serverConfig);

                    continue;
                }

                await RunServer(serverConfig);
            }
        }

        private static STask RunServer(ServerConfig serverConfig)
        {
            STaskCompletionSource = new STaskCompletionSource();

            ProcessHelper.Run("dotnet", $"Server.App.dll --Server {serverConfig.Id}", "../Bin");
            
            return STaskCompletionSource.Task;
        }

        private static async STask Create(ServerConfig serverConfig, bool runProcess = false)
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
                new ServerStartFinished {ServerId = serverConfig.Id}.Send(ManageServer);
            }

            Log.Debug($"Server:{serverConfig.ServerType} 启动完成!");
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            STaskCompletionSource = null;
            
            base.Dispose();
        }
    }
}