using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sining.Network
{
    public class BitCoinJsonRpcRequest : IJsonRpcRequest
    {
        [JsonProperty(PropertyName = "jsonrpc")] public const double JsonRpc = 2.0;
        [JsonProperty(PropertyName = "method")] public string Method;
        [JsonProperty(PropertyName = "params")]
        public readonly List<object> Params = new List<object>();
        [JsonProperty(PropertyName = "id")] public int Id;

        public void Init(string method, int id, params object[] @params)
        {
            Id = id;
            Method = method;
            Params.Clear();
            Params.AddRange(@params);
        }
    }
}