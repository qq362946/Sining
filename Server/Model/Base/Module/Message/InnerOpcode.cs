using Sining.Network;

namespace Sining.Message
{
	// 服务器启动完成
	[Message(InnerOpcode.ServerStartFinished)]
	public partial class ServerStartFinished : IMessage {}

}
namespace Sining.Message
{
	public static partial class InnerOpcode
	{
		 public const ushort ServerStartFinished = 106;
	}
}
