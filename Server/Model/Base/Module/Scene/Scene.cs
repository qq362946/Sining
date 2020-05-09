using Sining.Config;
using Sining.Event;
using Sining.Module;

namespace Sining
{
    public class Scene : Component
    {
        public int ServerId;
        public int SceneId;
        public int Zone;
        public NetInnerComponent NetInnerComponent;

        public override void Dispose()
        {
            if (IsDispose) return;

            base.Dispose();

            SceneId = 0;
            NetInnerComponent = null;
        }
    }
}