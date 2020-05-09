namespace Sining.Config
{
	[Config]
	public partial class CoinConfigData : AConfig<CoinConfig>
	{
		public static CoinConfigData Instance { get; private set; }

		public CoinConfigData()
		{
			Instance = this;
		}
	}
}
