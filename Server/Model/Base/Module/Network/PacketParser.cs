using System;
using System.Buffers;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Sining.DataStructure;
using Sining.Message;
using Sining.Tools;

namespace Sining.Network
{
    public class PacketParser
    { 
        public const int PacketBody = ushort.MaxValue * 16;
        public const int PacketLength = sizeof(int);
        private const int ProtocolCodeLength = sizeof(ushort);
        public const int PacketHeadLength = PacketLength + ProtocolCodeLength;
        public const int PacketSizeMax = PacketHeadLength + PacketBody;
        private readonly CircularBuffer _buffer;
        private readonly MemoryStream _channelMemoryStream;
        public int MessagePacketLength;
        public ushort MessageProtocolCode;
        public const int MemoryPoolMaxLength = 8192;
    
        public PacketParser(CircularBuffer buffer, MemoryStream channelMemoryStream)
        {
            _buffer = buffer;
            _channelMemoryStream = channelMemoryStream;
        }

        public void JsonParse(Stream stream)
        {
            using var memoryOwner = MemoryPool<byte>.Shared.Rent(MemoryPoolMaxLength);
            stream.Read(memoryOwner.Memory.Span);

            _channelMemoryStream.Seek(0, SeekOrigin.Begin);
            _channelMemoryStream.Write(memoryOwner.Memory.Span);
            _channelMemoryStream.Seek(0, SeekOrigin.Begin);
            _channelMemoryStream.SetLength(MemoryPoolMaxLength);
        }

        public bool Parse()
        {
            if (_buffer.Length < PacketHeadLength) return false;
    
            if (MessagePacketLength == 0 || MessageProtocolCode == 0)
            {
                _channelMemoryStream.Seek(0, SeekOrigin.Begin);
                _buffer.Read(_channelMemoryStream, PacketHeadLength);
    
                var buffer = _channelMemoryStream.GetBuffer();
                MessagePacketLength = BitConverter.ToInt32(buffer, 0);
                MessageProtocolCode = BitConverter.ToUInt16(buffer, PacketLength);
    
                _channelMemoryStream.Seek(0, SeekOrigin.Begin);
            }
    
            if (_buffer.Length < MessagePacketLength) return false;
    
            _buffer.Read(_channelMemoryStream, MessagePacketLength);
            _channelMemoryStream.Seek(0, SeekOrigin.Begin);
            _channelMemoryStream.SetLength(MessagePacketLength);
    
            return true;
        }
        public void Clear()
        {
            MessagePacketLength = 0;
            MessageProtocolCode = 0;
        }
    }
}