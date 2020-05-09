namespace Sining.Network
{
    public interface IMessage { }

    public interface IRequest : IMessage
    {
        int RpcId { get; set; }
        int SceneId { get; set; }
    }

    public interface IResponse : IMessage
    {
        int RpcId { get; set; }
        int ErrorCode { get; set; }
    }
    
    public class ErrorResponse : IResponse
    {
        public int RpcId { get; set; }
        public int ErrorCode { get; set; }
    }
}