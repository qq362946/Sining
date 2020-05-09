using Sining.Config;
using Sining.Event;
using Sining.Message;
using Sining.Tools;

namespace Sining.Module
{
    public class StartServerComponent : Component
    {
        public override void Dispose()
        {
            if (IsDispose) return;

            base.Dispose();
        }
    }
}