using Sining.Module;
using Sining.Network;

namespace Sining.Message
{
	// 服务器启动完成
	public partial class ServerStartFinished : IMessage 
	{
		public int ServerId { get; set; }
	}
	// 创建一个新的用户
	public partial class CreateUserRequest : IRequest 
	{
		public int RpcId { get; set; }
		public int SceneId { get; set; }
		public User User { get; set; }
	}
	public partial class CreateUserResponse : IResponse 
	{
		public int RpcId { get; set; }
		public int ErrorCode { get; set; }
	}
}
