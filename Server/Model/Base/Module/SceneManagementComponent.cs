using System.Collections.Generic;
using Sining.Config;
using Sining.Event;
using Sining.Module;
using Sining.Network;

namespace Sining
{
    [ComponentSystem]
    public class SceneManagementComponentAwakeSystem : AwakeSystem<SceneManagementComponent>
    {
        protected override void Awake(SceneManagementComponent self)
        {
            SceneManagementComponent.Instance = self;
        }
    }

    public class SceneManagementComponent : Component
    {
        public static SceneManagementComponent Instance;
        private readonly Dictionary<int, Scene> _scenes = new Dictionary<int, Scene>();

        public void Create(ServerConfig serverConfig, SceneConfig sceneConfig)
        {
            var sceneType = (SceneType) sceneConfig.Id;
            
            var scene =
                ComponentFactory.Create<Scene, SceneType, SceneConfig>(
                    SApp.Scene, sceneType, sceneConfig, this, true);
            
            scene.Scene = scene;
            _scenes.Add(sceneConfig.Id, scene);
            scene.AddDbComponent();
            
            // 挂载网络服务
            switch (sceneConfig.NetworkProtocol)
            {
                case "TCP" when !string.IsNullOrWhiteSpace(serverConfig.OuterIP) && sceneConfig.OuterPort > 0:
                    scene.AddComponent<NetOuterComponent, MessagePacker, string, string>(
                        ComponentFactory.Create<ProtobufMessagePacker>(scene),
                        $"{serverConfig.OuterIP}:{sceneConfig.OuterPort}",
                        sceneConfig.NetworkProtocol);
                    break;
                case "WebSocket" when sceneConfig.Urls.Length > 0:
                    scene.AddComponent<NetOuterComponent, MessagePacker, IEnumerable<string>, string>(
                        ComponentFactory.Create<ProtobufMessagePacker>(scene),
                        sceneConfig.Urls,
                        sceneConfig.NetworkProtocol);
                    break;
                case "HTTP" when sceneConfig.Urls.Length > 0:
                    scene.AddComponent<NetOuterComponent, MessagePacker, IEnumerable<string>, string>(
                        ComponentFactory.Create<JsonMessagePacker>(scene),
                        sceneConfig.Urls,
                        sceneConfig.NetworkProtocol);
                    break;
            }

            SceneFactory.Create(scene);
        }

        public Scene GetScene(int sceneId)
        {
            _scenes.TryGetValue(sceneId, out var scene);
            return scene;
        }
        public void Remove(int sceneId)
        {
            if (!_scenes.Remove(sceneId, out var scene))
            {
                return;
            }
            
            scene.Dispose();
        }
        public override void Dispose()
        {
            if (IsDispose) return;
            
            _scenes.Clear();
            
            base.Dispose();
        }
    }
}