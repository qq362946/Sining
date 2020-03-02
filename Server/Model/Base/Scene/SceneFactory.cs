using System;
using Sining.Config;
using Sining.Module;

namespace Sining
{
    public static class SceneFactory
    {
        public static void Create(SceneConfig sceneConfig)
        {
            var sceneType = (SceneType) sceneConfig.Id;
            var scene = App.Scene.AddComponent<Scene, SceneType, SceneConfig>(sceneType, sceneConfig);

            if (sceneConfig.OuterPort > 0 &&
                !string.IsNullOrWhiteSpace(sceneConfig.NetworkProtocol) &&
                !string.IsNullOrWhiteSpace(App.ServerConfig.OuterIP))
            {
                scene.AddComponent<NetOuterComponent, string, string>(
                    $"{App.ServerConfig.OuterIP}:{sceneConfig.OuterPort}",
                    sceneConfig.NetworkProtocol);
            }

            switch (sceneType)
            {
                case SceneType.RealmAccount:
                    break;
                case SceneType.MessageForwarding:
                    break;
                case SceneType.Map:
                    break;
                case SceneType.Location:
                    break;
                default:
                    throw new Exception($"No SceneType found for {sceneConfig.Id}");
            }
        }
    }
}