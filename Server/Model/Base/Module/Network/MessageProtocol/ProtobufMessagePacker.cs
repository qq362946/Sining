using System;
using System.IO;
using Sining.Tools;

namespace Sining.Network
{
    public class ProtobufMessagePacker : MessagePacker
    {
        public override byte[] SerializeTo<T>(T t)
        {
            return ProtobufHelper.SerializeTo(t);
        }

        public override void SerializeTo<T>(T t, MemoryStream stream)
        {
            ProtobufHelper.SerializeTo(t, stream);
        }

        public override object DeserializeFrom(Type type, MemoryStream stream)
        {
            return ProtobufHelper.DeserializeFrom(type, stream);
        }

        public override T DeserializeFrom<T>(byte[] bytes, int index, int count)
        {
            return ProtobufHelper.DeserializeFrom<T>(bytes, index, count);
        }

        public override T DeserializeFrom<T>(MemoryStream stream)
        {
            return ProtobufHelper.DeserializeFrom<T>(stream);
        }
    }
}