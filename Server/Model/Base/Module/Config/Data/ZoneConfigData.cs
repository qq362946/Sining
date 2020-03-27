namespace Sining.Config
{
	[Config]
	public partial class ZoneConfigData : AConfig<ZoneConfig>
	{
		public static ZoneConfigData Instance { get; private set; }

		public ZoneConfigData()
		{
			Instance = this;
		}
	}
}
