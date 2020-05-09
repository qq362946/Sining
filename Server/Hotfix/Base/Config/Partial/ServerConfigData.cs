using System.Collections.Generic;
using Sining.DataStructure;

namespace Sining.Config
{
    public partial class ServerConfigData
    {
        public readonly OneToManyList<int, ServerConfig> Servers = new OneToManyList<int, ServerConfig>();
        public static readonly List<ServerConfig> UserServers = new List<ServerConfig>();

        public override void BeginInit()
        {
            base.BeginInit();

            foreach (var configsValue in Configs.Values)
            {
                Servers.Add(configsValue.ServerType, configsValue);

                if (configsValue.ServerType == (int) ServerType.User)
                {
                    UserServers.Add(configsValue);
                }
            }
        }

        public List<ServerConfig> GetServers(ServerType serverType)
        {
            Servers.TryGetValue((int) serverType, out var list);

            return list;
        }
    }
}