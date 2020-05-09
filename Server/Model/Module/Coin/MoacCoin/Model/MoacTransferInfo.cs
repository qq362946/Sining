using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sining.Module
{
    public class MoacTransferInfo
    {
        [JsonProperty("transactionHash")] public string TransactionHash;
        [JsonProperty("transactionIndex")] public string TransactionIndex;
        [JsonProperty("blockHash")] public string BlockHash;
        [JsonProperty("blockNumber")] public string BlockNumber;
        [JsonProperty("cumulativeGasUsed")] public string CumulativeGasUsed;
        [JsonProperty("gasUsed")] public string GasUsed;
        [JsonProperty("contractAddress")] public string ContractAddress;
        [JsonProperty("status")] public string Status;
        [JsonProperty("logs")] public List<MoacTransferInfoLogs> Logs;
    }
    
    public class MoacTransferInfoLogs
    {
        [JsonProperty("address")] public string Address;
        [JsonProperty("topics")] public List<string> Topics;
        [JsonProperty("TxData")] public string TxData;
        [JsonProperty("blockNumber")] public string BlockNumber;
        [JsonProperty("transactionHash")] public string TransactionHash;
        [JsonProperty("transactionIndex")] public string TransactionIndex;
        [JsonProperty("blockHash")] public string BlockHash;
        [JsonProperty("logIndex")] public string LogIndex;
        [JsonProperty("removed")] public bool Removed;
    }
}