namespace Sining.Config
{
	[Config]
	public partial class FinancingConfigData : AConfig<FinancingConfig>
	{
		public static FinancingConfigData Instance { get; private set; }

		public FinancingConfigData()
		{
			Instance = this;
		}
	}
}
