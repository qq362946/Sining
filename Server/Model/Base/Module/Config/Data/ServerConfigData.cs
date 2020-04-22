namespace Sining.Config
{
	[Config]
	public partial class ServerConfigData : AConfig<ServerConfig>
	{
		public static ServerConfigData Instance { get; private set; }

		public ServerConfigData()
		{
			Instance = this;
		}
	}
}
