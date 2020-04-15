using System.Collections.Generic;
using Sining.Config;
using Sining.Module;
using Sining.Network;

namespace Sining
{
    public static class SceneManagementComponentSystem
    {
        public static async STask Create(this SceneManagementComponent self, ServerConfig serverConfig,
            SceneConfig sceneConfig)
        {
            var sceneType = (SceneType) sceneConfig.Id;

            var scene =
                ComponentFactory.Create<Scene, SceneType, SceneConfig>(
                    SApp.Scene, sceneType, sceneConfig, self, true);

            scene.Scene = scene;
            self.Scenes.Add(sceneConfig.Id, scene);
            scene.AddDbComponent();

            // 挂载网络服务
            switch (sceneConfig.NetworkProtocol)
            {
                case "TCP" when !string.IsNullOrWhiteSpace(serverConfig.OuterIP) && sceneConfig.OuterPort > 0:
                    scene.AddComponent<NetOuterComponent, MessagePacker, string, string>(
                        ComponentFactory.Create<ProtobufMessagePacker>(scene),
                        $"{serverConfig.OuterIP}:{sceneConfig.OuterPort}",
                        sceneConfig.NetworkProtocol);
                    scene.GetComponent<NetOuterComponent>().MessageDispatcher = new OuterMessageDispatcher();
                    break;
                case "WebSocket" when sceneConfig.Urls.Length > 0:
                    scene.AddComponent<NetOuterComponent, MessagePacker, IEnumerable<string>, string>(
                        ComponentFactory.Create<ProtobufMessagePacker>(scene),
                        sceneConfig.Urls,
                        sceneConfig.NetworkProtocol);
                    scene.GetComponent<NetOuterComponent>().MessageDispatcher = new OuterMessageDispatcher();
                    break;
                case "HTTP" when sceneConfig.Urls.Length > 0:
                    scene.AddComponent<NetOuterComponent, MessagePacker, IEnumerable<string>, string>(
                        ComponentFactory.Create<JsonMessagePacker>(scene),
                        sceneConfig.Urls,
                        sceneConfig.NetworkProtocol);
                    break;
            }

            await SceneFactory.Create(scene);
        }
    }
}