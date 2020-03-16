using System;
using System.IO;
using System.Text;
using Server.Network;
using Sining.Message;
using Sining.Module;
using Sining.Tools;

namespace Sining.Network
{
    public class JsonMessagePacker : MessagePacker
    {
        public override string SerializeToJson<T>(T t)
        {
            return t.Serialize();
        }

        public override byte[] SerializeTo<T>(T t)
        {
            return t.SerializeToByte();
        }

        public override void SerializeTo<T>(T t, MemoryStream stream)
        {
            throw new NotImplementedException();
        }

        public override T DeserializeFrom<T>(string json)
        {
            return json.Deserialize<T>();
        }

        public override object DeserializeFrom(Type type, MemoryStream stream)
        {
            return Encoding.UTF8.GetString(stream.GetBuffer()).Deserialize(type);
        }

        public override T DeserializeFrom<T>(byte[] bytes, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override T DeserializeFrom<T>(MemoryStream stream)
        {
            throw new NotImplementedException();
        }

        public override void Unpack(Session session, IMessage message, NetworkComponent network,
            ref MemoryStream memoryStream)
        {
            if (message == null)
            {
                throw new Exception("message cannot be null");
            }

            if (session.IsDispose)
            {
                throw new Exception("session has been Disposed");
            }

            var json = SerializeToJson(message);

            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.SetLength(0);
            memoryStream.Write(Encoding.UTF8.GetBytes(json));
            memoryStream.Seek(0, SeekOrigin.Begin);
        }
    }
}