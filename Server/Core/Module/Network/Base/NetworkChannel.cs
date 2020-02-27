using System.IO;

namespace Sining.Network
{
    public abstract class NetworkChannel : Component
    {
        public MemoryStream MemoryStream;
        public string RemoteAddress;
        public abstract void Send(MemoryStream memoryStream);
    }
}