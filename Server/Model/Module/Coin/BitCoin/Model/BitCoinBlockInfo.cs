using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sining.Model
{
    public class BitCoinBlockInfo
    {
        [JsonProperty("hash")] public string Hash;
        [JsonProperty("confirmations")] public int Confirmations;
        [JsonProperty("strippedsize")] public int Strippedsize;
        [JsonProperty("size")] public int Size;
        [JsonProperty("weight")] public int Weight;
        [JsonProperty("height")] public int Height;
        [JsonProperty("version")] public int Version;
        [JsonProperty("versionHex")] public string VersionHex;
        [JsonProperty("merkleroot")] public string Merkleroot;
        [JsonProperty("tx")] public List<string> Tx;
        [JsonProperty("time")] public int Time;
        [JsonProperty("mediantime")] public int Mediantime;
        [JsonProperty("nonce")] public int Nonce;
        [JsonProperty("bits")] public string Bits;
        [JsonProperty("difficulty")] public int Difficulty;
        [JsonProperty("chainwork")] public string Chainwork;
        [JsonProperty("previousblockhash")] public string PreviousBlockHash;
        [JsonProperty("nextblockhash")] public string NextBlockHash;
    }
}