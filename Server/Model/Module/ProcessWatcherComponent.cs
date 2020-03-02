using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Sining.Config;
using Sining.DataStructure;
using Sining.Event;
using Sining.Tools;

namespace Sining.Module
{
    [ComponentSystem]
    public class ProcessWatcherComponentAwakeSystem : AwakeSystem<ProcessWatcherComponent>
    {
        protected override void Awake(ProcessWatcherComponent self)
        {
            self.Awake();
        }
    }

    public class ProcessWatcherComponent : Component
    {
        private Process RunServer(int serverId)
        {
            return ProcessHelper.Run("dotnet", $"Server.App.dll --Server {serverId}", "../Bin");
        }

        public void Awake()
        {
            foreach (var serverConfig in ServerConfigData.Instance.GetAllConfig())
            {
                RunServer(serverConfig.Id);
            }
        }
    }
}