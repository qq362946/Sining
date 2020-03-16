using Sining.Message;

namespace Sining.Network.Actor
{
    [Message(Opcode.ActorResponse)]
    public class ActorResponse : IActorResponse
    {
        public int RpcId { get; set; }
        public int ErrorCode { get; set; }
    }
}