using Sining.Module;

namespace Sining.Network
{
    public abstract class NetworkProtocol : Component
    {
        public abstract NetworkChannel GetChannel(long channelId);

        public abstract NetworkChannel ConnectChannel(Session session, string address);
        public abstract void RemoveChannel(long channelId);
    }
}