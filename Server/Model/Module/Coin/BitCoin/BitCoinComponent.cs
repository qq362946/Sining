using System;
using System.Net.Http.Headers;
using System.Text;

namespace Sining.Module
{
    public class BitCoinComponent : Component
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