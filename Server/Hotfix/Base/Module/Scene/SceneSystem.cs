using Sining.Config;
using Sining.Event;
using Sining.Module;

namespace Sining
{
    [ComponentSystem]
    public class SceneAwakeSystem : AwakeSystem<Scene, int, SceneConfig, NetInnerComponent>
    {
        protected override void Awake(Scene self, int serverId, SceneConfig sceneConfig, NetInnerComponent inner)
        {
            self.Awake(serverId, sceneConfig, inner);
        }
    }

    [ComponentSystem]
    public class ServerSceneAwakeSystem : AwakeSystem<Scene, ServerConfig>
    {
        protected override void Awake(Scene self, ServerConfig serverConfig)
        {
            self.ServerId = serverConfig.Id;
        }
    }
    
    public static class SceneSystem
    {
        public static void Awake(this Scene self, int serverId, SceneConfig sceneConfig, NetInnerComponent inner = null)
        {
            self.ServerId = serverId;
            self.SceneId = sceneConfig.Id;
            self.Zone = sceneConfig.Zone;

            if (inner != null)
            {
                self.NetInnerComponent = inner;
            }

            self.Scene = self;
        }

        public static SceneConfig GetSceneConfig(this Scene self)
        {
            return SceneConfigData.Instance.GetConfig(self.SceneId);
        }

        public static void LoadSceneConfig(this Scene self, int serverId, int sceneConfigId)
        {
            var sceneConfig = SceneConfigData.Instance.GetConfig(sceneConfigId);
            self.Awake(serverId, sceneConfig);
        }
    }
}