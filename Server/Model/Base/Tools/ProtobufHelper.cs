using System;
using System.IO;
using Google.Protobuf;

namespace Sining.Tools
{
    public static class ProtobufHelper
    {
        public static byte[] SerializeTo<T>(T t)
        {
            return ((IMessage) t).ToByteArray();
        }

        public static void SerializeTo<T>(T t, MemoryStream stream)
        {
            ((IMessage) t).WriteTo(stream);
        }

        public static object DeserializeFrom(Type type, MemoryStream stream)
        {
            var message = Activator.CreateInstance(type);

            ((IMessage) message).MergeFrom(stream.GetBuffer(), (int) stream.Position,
                (int) stream.Length);

            return message;
        }

        public static T DeserializeFrom<T>(byte[] bytes, int index, int count)
        {
            var message = Activator.CreateInstance(typeof(T));

            ((IMessage) message).MergeFrom(bytes, index, count);

            return (T) message;
        }

        public static T DeserializeFrom<T>(MemoryStream stream)
        {
            return (T) DeserializeFrom(typeof(T), stream);
        }
    }
}