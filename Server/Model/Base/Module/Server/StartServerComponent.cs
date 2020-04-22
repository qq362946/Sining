using Sining.Config;
using Sining.Event;
using Sining.Message;
using Sining.Tools;

namespace Sining.Module
{
    public class StartServerComponent : Component
    {
        public static STaskCompletionSource STaskCompletionSource;
        public string ManageServer = "127.0.0.1:8999";
        public override void Dispose()
        {
            if (IsDispose) return;

            STaskCompletionSource = null;
            
            base.Dispose();
        }
    }
}