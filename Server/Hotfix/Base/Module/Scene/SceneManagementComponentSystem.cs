using System.Collections.Generic;
using Sining.Config;
using Sining.Module;
using Sining.Network;

namespace Sining
{
    public static class SceneManagementComponentSystem
    {
        /// <summary>
        /// 创建一个服务器Scene（一个服务器只有一个主Scene）
        /// </summary>
        /// <param name="self"></param>
        /// <param name="serverConfig"></param>
        /// <returns></returns>
        public static Scene CreateServerScene(this SceneManagementComponent self,
            ServerConfig serverConfig)
        {
            if (self.ServerScenes.TryGetValue(serverConfig.Id, out var serverScene)) return serverScene;

            serverScene =
                ComponentFactory.Create<Scene, ServerConfig>(
                    MainScene.Scene, serverConfig, self, true);
            self.ServerScenes.Add(serverConfig.Id, serverScene);

            // 挂载内网通讯组件

            serverScene.NetInnerComponent =
                serverScene.AddComponent<NetInnerComponent, string>(
                    $"{serverConfig.InnerIP}:{serverConfig.InnerPort}");

            return serverScene;
        }

        /// <summary>
        /// 创建一个Scene（挂载到服务器Scene下）
        /// </summary>
        /// <param name="self"></param>
        /// <param name="serverScene"></param>
        /// <param name="serverConfig"></param>
        /// <param name="sceneConfig"></param>
        /// <returns></returns>
        public static async STask Create(this SceneManagementComponent self,
            Scene serverScene,
            ServerConfig serverConfig,
            SceneConfig sceneConfig)
        {
            var isExist = true;

            if (!self.Scenes.TryGetValue(sceneConfig.Id, out var scene))
            {
                scene =
                    ComponentFactory.Create<Scene, int, SceneConfig, NetInnerComponent>(
                        serverScene, serverConfig.Id, sceneConfig, serverScene.NetInnerComponent, self, true);

                isExist = false;
                self.Scenes.Add(sceneConfig.Id, scene);
                scene.InitDbComponent();
            }

            // 挂载网络服务

            switch (sceneConfig.NetworkProtocol)
            {
                case "TCP" when !string.IsNullOrWhiteSpace(serverConfig.OuterIP) && sceneConfig.OuterPort > 0:
                {
                    var outer = ComponentFactory.Create<NetOuterComponent, MessagePacker, string, string>(
                        scene,
                        ComponentFactory.Create<ProtobufMessagePacker>(scene),
                        $"{serverConfig.OuterIP}:{sceneConfig.OuterPort}",
                        sceneConfig.NetworkProtocol, scene, true);

                    outer.MessageDispatcher = new OuterMessageDispatcher();
                    break;
                }
                case "WebSocket" when sceneConfig.Urls.Length > 0:
                {
                    var outer = ComponentFactory.Create<NetOuterComponent, MessagePacker, IEnumerable<string>, string>(
                        scene,
                        ComponentFactory.Create<ProtobufMessagePacker>(scene),
                        sceneConfig.Urls,
                        sceneConfig.NetworkProtocol, scene, true);

                    outer.MessageDispatcher = new OuterMessageDispatcher();
                    break;
                }
                case "HTTP" when sceneConfig.Urls.Length > 0:
                {
                    ComponentFactory.Create<NetOuterComponent, MessagePacker, IEnumerable<string>, string>(
                        scene,
                        ComponentFactory.Create<JsonMessagePacker>(scene),
                        sceneConfig.Urls,
                        sceneConfig.NetworkProtocol, scene, true);
                    break;
                }
            }

            if (!isExist)
            {
                await SceneFactory.Create(scene);
            }
        }
    }
}