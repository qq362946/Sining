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
        public readonly Dictionary<int, Scene> Scenes = new Dictionary<int, Scene>();
        public readonly Dictionary<int, Scene> ServerScenes = new Dictionary<int, Scene>();
        public Scene GetScene(int sceneId)
        {
            Scenes.TryGetValue(sceneId, out var scene);
            return scene;
        }
        public Scene GetServerScene(int serverId)
        {
            ServerScenes.TryGetValue(serverId, out var scene);
            return scene;
        }
        public void RemoveScene(int sceneId)
        {
            if (!Scenes.Remove(sceneId, out var scene))
            {
                return;
            }
            
            scene.Dispose();
        }
        public void RemoveServerScene(int serverId)
        {
            if (!ServerScenes.Remove(serverId, out var scene))
            {
                return;
            }
            
            scene.Dispose();
        }
        public override void Dispose()
        {
            if (IsDispose) return;
            
            Scenes.Clear();
            ServerScenes.Clear();
            
            base.Dispose();
        }
    }
}