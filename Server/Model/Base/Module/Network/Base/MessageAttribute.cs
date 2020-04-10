using Sining.Event;

namespace Sining.Network
{
    public class MessageAttribute : BaseAttribute
    {
        public ushort Opcode { get; }
        public MessageAttribute(ushort opcode)
        {
            Opcode = opcode;
        }
    }
}