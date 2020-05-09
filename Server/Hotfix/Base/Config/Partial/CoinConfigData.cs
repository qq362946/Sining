namespace Sining.Config
{
    public partial class CoinConfigData
    {
        public CoinConfig GetCoinConfig(CoinConfigType coinConfigType)
        {
            return GetConfig((int) coinConfigType);
        }
    }
}