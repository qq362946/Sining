using Newtonsoft.Json;

namespace Sining.Network
{
    public class BitCoinJsonRpcResponse<T>
    {
        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }
}