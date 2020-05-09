using System.Linq;
using Sining.Config;
using Sining.Event;

namespace Sining.Module
{
    [ComponentSystem]
    public class StartMultiProgressComponentAwakeSystem : AwakeSystem<StartMultiProgressComponent>
    {
        protected override void Awake(StartMultiProgressComponent self)
        {
            Log.Debug("[Multi Mode]");
            Log.Debug("Start the server, please wait...");

            AwakeAsync(self).Coroutine();
        }

        private async SVoid AwakeAsync(StartMultiProgressComponent self)
        {
            var servers = ServerConfigData.Instance.Servers.Where(d => d.Key != MainScene.Id);
            
            foreach (var server in servers)
            {
                foreach (var serverConfig in server.Value)
                {
                    await ServerHelper.Start(serverConfig.Id, false);
                }
            }
        }
    }
}