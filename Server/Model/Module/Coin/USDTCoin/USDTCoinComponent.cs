using System.Net.Http.Headers;

namespace Sining.Module
{
    public class USDTCoinComponent : Component
    {
        public string Url;
        public string NodeName;
        public int JsonId;
        public AuthenticationHeaderValue Authentication;

        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }
            
            base.Dispose();

            Url = null;
            NodeName = null;
            Authentication = null;
            JsonId = 0;
        }
    }
}