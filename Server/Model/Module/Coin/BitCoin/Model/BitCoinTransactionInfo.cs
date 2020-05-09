using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sining.Model
{
    public class BitCoinTransactionInfo
    {
        [JsonProperty("account")] public string Account;
        [JsonProperty("address")] public string Address;
        [JsonProperty("category")] public string Category;
        [JsonProperty("amount")] public double Amount;
        [JsonProperty("label")] public string Label;
        [JsonProperty("vout")] public int Vout;
        [JsonProperty("confirmations")] public int Confirmations;
        [JsonProperty("blockhash")] public string BlockHash;
        [JsonProperty("blockindex")] public int BlockIndex;
        [JsonProperty("blocktime")] public int BlockTime;
        [JsonProperty("txid")] public string TxId;
        [JsonProperty("walletconflicts")] public List<string> WalletConflicts;
        [JsonProperty("time")] public int Time;
        [JsonProperty("timereceived")] public int TimeReceived;
        [JsonProperty("bip125-replaceable")] public string Bip125Replaceable;
        [JsonProperty("comment")] public string Comment;
        [JsonProperty("to")] public string To;
    }
}