using Newtonsoft.Json;

namespace Sining.Module
{
    public class UsdtBalanceInfo
    {
        [JsonProperty("balance")]public string Balance;
        [JsonProperty("reserved")]public string Reserved;
        [JsonProperty("frozen")]public string Rrozen;
    }
}