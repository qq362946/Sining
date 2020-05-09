using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sining.Model
{
    public class BitTransactionInfoDetails
    {
        [JsonProperty("account")] public string Account;
        [JsonProperty("address")] public string Address;
        [JsonProperty("category")] public string Category;
        [JsonProperty("amount")] public double Amount;
        [JsonProperty("vout")] public int Vout;
        [JsonProperty("fee")] public int Fee;
    }

    public class BitTransactionInfo
    {
        [JsonProperty("amount")] public int Amount;
        [JsonProperty("fee")] public int Fee;
        [JsonProperty("confirmations")] public int Confirmations;
        [JsonProperty("blockhash")] public string BlockHash;
        [JsonProperty("blockindex")] public int BlockIndex;
        [JsonProperty("blocktime")] public int BlockTime;
        [JsonProperty("txid")] public string TxId;
        [JsonProperty("walletconflicts")] public List<string> WalletConflicts;
        [JsonProperty("time")] public int Time;
        [JsonProperty("timereceived")] public int TimeReceived;
        [JsonProperty("bip125-replaceable")] public string Bip125_Replaceable;
        [JsonProperty("details")] public List<BitTransactionInfoDetails> Details;
        [JsonProperty("hex")] public string Hex;
    }
}