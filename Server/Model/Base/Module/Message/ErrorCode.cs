namespace Sining.Network
{
    public partial class ErrorCode
    {
        public const int ErrSessionDispose = 1;  // Session已经被释放掉了
        public const int ErrRpcFail = 2;         // Rpc消息发送失败
        public const int ErrActorTimeout = 3;    // 发送Actor消息超时
    }
}