using System;
using System.IO;
using Server.Network;
using Sining.Module;
using Sining.Tools;

namespace Sining.Network
{
    public class ProtobufMessagePacker : MessagePacker
    {
        public override string SerializeToJson<T>(T t)
        {
            return t.ToJson();
        }

        public override byte[] SerializeTo<T>(T t)
        {
            return t.ToBytes();
        }

        public override void SerializeTo<T>(T t, MemoryStream stream)
        {
            ProtobufHelper.SerializeTo(t, stream);
        }

        public override T DeserializeFrom<T>(string json)
        {
            throw new NotImplementedException();
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

            var opCode = NetworkProtocolManagement.Instance.GetOpCode(message.GetType());

            memoryStream.Seek(PacketParser.PacketHeadLength, SeekOrigin.Begin);
            memoryStream.SetLength(PacketParser.PacketHeadLength);
            network.MessagePacker.SerializeTo(message, memoryStream);

            var byteLength = memoryStream.Length - PacketParser.PacketHeadLength;
            if (byteLength > PacketParser.PacketBody)
            {
                throw new Exception($"Message content exceeds {PacketParser.PacketBody} bytes");
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.Write(BitConverter.GetBytes((int) byteLength));
            memoryStream.Write(BitConverter.GetBytes(opCode));
            memoryStream.Seek(0, SeekOrigin.Begin);
        }
    }
}