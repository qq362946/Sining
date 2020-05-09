using Newtonsoft.Json;

namespace Sining.Module
{
    public class UsdtTransactionInfo
    {
        [JsonProperty("txid")] public string Txid;
        [JsonProperty("fee")] public string Fee;
        [JsonProperty("sendingaddress")] public string SendingAddress;
        [JsonProperty("referenceaddress")] public string ReferenceAddress;
        [JsonProperty("ismine")] public bool IsMine;
        [JsonProperty("version")] public int Version;
        [JsonProperty("type_int")] public int Type_Int;
        [JsonProperty("type")] public string Type;
        [JsonProperty("propertyid")] public int PropertyId;
        [JsonProperty("divisible")] public bool Divisible;
        [JsonProperty("amount")] public string Amount;
        [JsonProperty("valid")] public bool Valid;
        [JsonProperty("blockhash")] public string BlockHash;
        [JsonProperty("blocktime")] public int BlockTime;
        [JsonProperty("positioninblock")] public int PositioninBlock;
        [JsonProperty("block")] public int Block;
        [JsonProperty("confirmations")] public int Confirmations;
    }
}