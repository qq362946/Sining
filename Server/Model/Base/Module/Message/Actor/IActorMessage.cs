namespace Sining.Network
{
    public interface IActorMessage: IMessage
    {
        long ActorId { get; set; }
    }

    public interface IActorRequest : IActorMessage, IRequest { }

    public interface IActorResponse : IResponse { }
}