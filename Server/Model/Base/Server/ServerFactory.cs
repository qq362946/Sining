using System;
using Sining.Config;
using Sining.Module;

namespace Sining
{
    public static class ServerFactory
    {
        public static void Create()
        {
            if (!string.IsNullOrWhiteSpace(App.ServerConfig.InnerIP) &&
                App.ServerConfig.InnerPort > 0)
            {
                App.Scene.AddComponent<NetInnerComponent, string>(
                    $"{App.ServerConfig.InnerIP}:{App.ServerConfig.InnerPort}");
            }

            var scenes = SceneConfigData.Instance.GetByServer(App.Id);

            foreach (var sceneConfig in scenes)
            {
                SceneFactory.Create(sceneConfig);
            }
        }
    }
}