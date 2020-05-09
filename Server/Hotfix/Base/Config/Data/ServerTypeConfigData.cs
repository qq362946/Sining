namespace Sining.Config
{
	[Config]
	public partial class ServerTypeConfigData : AConfig<ServerTypeConfig>
	{
		public static ServerTypeConfigData Instance { get; private set; }

		public ServerTypeConfigData()
		{
			Instance = this;
		}
	}
}
