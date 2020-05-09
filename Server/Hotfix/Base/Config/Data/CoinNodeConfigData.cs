namespace Sining.Config
{
	[Config]
	public partial class CoinNodeConfigData : AConfig<CoinNodeConfig>
	{
		public static CoinNodeConfigData Instance { get; private set; }

		public CoinNodeConfigData()
		{
			Instance = this;
		}
	}
}
