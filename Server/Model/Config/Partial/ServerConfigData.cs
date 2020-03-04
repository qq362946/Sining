using System;
using System.Collections.Generic;
using Sining.DataStructure;

namespace Sining.Config
{
    public partial class ServerConfigData
    {
        private readonly OneToManyList<int, ServerConfig> _servers = new OneToManyList<int, ServerConfig>();

        public override void BeginInit()
        {
            base.BeginInit();

            foreach (var configsValue in Configs.Values)
            {
                _servers.Add((int) Enum.Parse(typeof(ServerType), configsValue.ServerType), configsValue);
            }
        }

        public List<ServerConfig> GetServers(ServerType serverType)
        {
            _servers.TryGetValue((int) serverType, out var list);

            return list;
        }
    }
}