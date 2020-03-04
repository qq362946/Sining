using System.IO;
using Sining.Module;

namespace Sining.Network
{
    public abstract class NetworkChannel : Component
    {
        public MemoryStream MemoryStream;
        public string RemoteAddress;
        public abstract void Send(Session session, MemoryStream memoryStream);
    }
}