using System.Linq;
using Sining.Config;
using Sining.Event;

namespace Sining.Module
{
    [ComponentSystem]
    public class StartSingleProcessComponentAwakeSystem : AwakeSystem<StartSingleProcessComponent>
    {
        protected override void Awake(StartSingleProcessComponent self)
        {
            Log.Debug("[Single Mode]");
            Log.Debug("Start the server, please wait...");
            AwakeAsync().Coroutine();
        }

        private async SVoid AwakeAsync()
        {
            var servers = ServerConfigData.Instance.Servers.Where(d => d.Key != MainScene.Id);
            
            foreach (var server in servers)
            {
                foreach (var serverConfig in server.Value)
                {
                    await ServerHelper.Start(serverConfig.Id);
                }
            }
        }
    }
}