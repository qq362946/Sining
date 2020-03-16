using Sining.Event;

namespace Sining.Network
{
    public class MessageAttribute : BaseAttribute
    {
        public ushort Opcode { get; }
        public string RawUrl { get; }

        public MessageAttribute(ushort opcode)
        {
            Opcode = opcode;
        }
        
        public MessageAttribute(ushort opcode,string rawUrl)
        {
            Opcode = opcode;
            RawUrl = rawUrl;
        }
    }
}