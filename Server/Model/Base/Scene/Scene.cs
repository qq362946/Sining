using Sining.Config;
using Sining.Event;

namespace Sining
{
    [ComponentSystem]
    public class SceneAwakeSystem : AwakeSystem<Scene, SceneType, SceneConfig>
    {
        protected override void Awake(Scene self, SceneType sceneType, SceneConfig sceneConfig)
        {
            self.Awake(sceneType, sceneConfig);
        }
    }

    public class Scene : Component
    {
        public SceneConfig SceneConfig { get; private set; }
        public SceneType SceneType { get; private set; }

        public void Awake(SceneType sceneType, SceneConfig sceneConfig)
        {
            SceneType = sceneType;
            SceneConfig = sceneConfig;
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            SceneType = SceneType.None;
            
            base.Dispose();
        }
    }
}