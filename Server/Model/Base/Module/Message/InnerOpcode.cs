using Sining.Network;

namespace Sining.Message
{
	// 服务器启动完成
	[Message(InnerOpcode.ServerStartFinished)]
	public partial class ServerStartFinished : IMessage {}

	// 创建一个新的用户
	[Message(InnerOpcode.CreateUserRequest)]
	public partial class CreateUserRequest : IRequest {}

	[Message(InnerOpcode.CreateUserResponse)]
	public partial class CreateUserResponse : IResponse {}

}
namespace Sining.Message
{
	public static partial class InnerOpcode
	{
		 public const ushort ServerStartFinished = 109;
		 public const ushort CreateUserRequest = 110;
		 public const ushort CreateUserResponse = 111;
	}
}
