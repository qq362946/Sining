using System;
using System.IO;
using Sining.Module;

namespace Sining.Network
{
    public abstract class MessagePacker : Component
    {
        public abstract string SerializeToJson<T>(T t);
        public abstract byte[] SerializeTo<T>(T t);
        public abstract void SerializeTo<T>(T t, MemoryStream stream);
        public abstract T DeserializeFrom<T>(string json);
        public abstract object DeserializeFrom(Type type, MemoryStream stream);
        public abstract T DeserializeFrom<T>(byte[] bytes, int index, int count);
        public abstract T DeserializeFrom<T>(MemoryStream stream);
        public abstract void Unpack(Session session, IMessage message, NetworkComponent network,
            ref MemoryStream memoryStream);
    }
}