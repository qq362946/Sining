using System.Collections.Generic;
using Sining.DataStructure;

namespace Sining.Config
{
    public partial class SceneConfigData
    {
        private readonly OneToManyList<int, SceneConfig> _servers = new OneToManyList<int, SceneConfig>();
        private readonly OneToManyList<int, SceneConfig> _zones = new OneToManyList<int, SceneConfig>();
        
        public override void BeginInit()
        {
            base.BeginInit();

            foreach (var (_, value) in Configs)
            {
                _servers.Add(value.ServerType, value);
                _zones.Add(value.Zone, value);
            }
        }

        public List<SceneConfig> GetByServerType(int serverId)
        {
            _servers.TryGetValue(serverId, out var list);
            return list;
        }
        
        public List<SceneConfig> GetByZone(int zoneId)
        {
            _zones.TryGetValue(zoneId, out var list);
            return list;
        }
    }
}