using System;
using System.IO;

namespace Sining.Core.Message
{
    public abstract class AMessagePacker : Component
    {
        public abstract byte[] SerializeTo<T>(T t);
        public abstract void SerializeTo<T>(T t, MemoryStream stream);
        public abstract T DeserializeFrom<T>(byte[] bytes, int index, int count);
        public abstract object DeserializeFrom(object instance, byte[] bytes, int index, int count);
        public abstract T DeserializeFrom<T>(MemoryStream stream);
        public abstract object DeserializeFrom(object instance, MemoryStream stream);
    }
}