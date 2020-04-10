using Sining.Network;

namespace Sining.Message
{
	// 测试框架协议
	[Message(OuterOpcode.TestMessage)]
	public partial class TestMessage : IMessage {}

	// 登录协议
	[Message(OuterOpcode.LoginRequest)]
	public partial class LoginRequest : IMessage {}

	// 注册协议
	[Message(OuterOpcode.RegRequest)]
	public partial class RegRequest : IMessage {}

	// 获取一个值
	[Message(OuterOpcode.GetNameRequest)]
	public partial class GetNameRequest : IRequest {}

	[Message(OuterOpcode.GetNameResponse)]
	public partial class GetNameResponse : IResponse {}

	// 测试Actor协议
	[Message(OuterOpcode.TestActorMessage)]
	public partial class TestActorMessage : IActorMessage {}

	// 测试ActorCall协议
	[Message(OuterOpcode.TestActorCallRequestMessage)]
	public partial class TestActorCallRequestMessage : IActorRequest {}

	[Message(OuterOpcode.TestActorCallResponseMessage)]
	public partial class TestActorCallResponseMessage : IActorResponse {}

}
namespace Sining.Message
{
	public static partial class OuterOpcode
	{
		 public const ushort TestMessage = 101;
		 public const ushort LoginRequest = 102;
		 public const ushort RegRequest = 103;
		 public const ushort GetNameRequest = 104;
		 public const ushort GetNameResponse = 105;
		 public const ushort TestActorMessage = 106;
		 public const ushort TestActorCallRequestMessage = 107;
		 public const ushort TestActorCallResponseMessage = 108;
	}
}
