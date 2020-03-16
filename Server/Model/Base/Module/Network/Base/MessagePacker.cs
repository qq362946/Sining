using System;
using System.IO;

namespace Sining.Network
{
    public abstract class MessagePacker : Component
    {
        public abstract byte[] SerializeTo<T>(T t);
        public abstract void SerializeTo<T>(T t, MemoryStream stream);

        public abstract object DeserializeFrom(Type type, MemoryStream stream);
        public abstract T DeserializeFrom<T>(byte[] bytes, int index, int count);
        public abstract T DeserializeFrom<T>(MemoryStream stream);
    }
}