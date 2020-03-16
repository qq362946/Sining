using Sining.Event;
using Sining.Module;
using System.Collections.Generic;

namespace Sining.Network
{
    [ComponentSystem]
    public class HttpComponentAcceptAwakeSystem : AwakeSystem<HttpComponent, IEnumerable<string>>
    {
        protected override void Awake(HttpComponent self, IEnumerable<string> urls)
        {
            self.Awake(urls);
        }
    }

    public class HttpComponent : NetworkProtocol
    {
        public void Awake(IEnumerable<string> urls)
        {
            AddComponent<HttpServerChannelComponent, IEnumerable<string>>(urls);
        }
        public override NetworkChannel GetChannel(long channelId)
        {
            return default;
        }

        public override NetworkChannel ConnectChannel(Session session, string address)
        {
            return null;
        }

        public override void RemoveChannel(long channelId) { }
    }
}