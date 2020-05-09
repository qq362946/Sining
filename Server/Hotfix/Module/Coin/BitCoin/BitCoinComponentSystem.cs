using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Sining.Model;
using Sining.Network;

namespace Sining.Module
{
    public static class BitCoinComponentSystem
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="self"></param>
        /// <param name="url"></param>
        /// <param name="jsonId"></param>
        /// <param name="nodeName"></param>
        public static void Init(this BitCoinComponent self, string url, int jsonId, string nodeName)
        {
            self.Init(url, jsonId, "", "", nodeName);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="self"></param>
        /// <param name="url"></param>
        /// <param name="jsonId"></param>
        /// <param name="user"></param>
        /// <param name="passWord"></param>
        /// <param name="nodeName"></param>
        public static void Init(this BitCoinComponent self, string url, int jsonId, string user, string passWord,
            string nodeName)
        {
            self.Url = url;
            self.NodeName = nodeName;
            self.JsonId = jsonId;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(passWord))
            {
                return;
            }

            self.Authentication = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{user}:{passWord}")
                ));
        }

        /// <summary>
        /// 创建一个新的账号
        /// </summary>
        /// <param name="self"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async STask<(string address, string privateKey)> CreateAccount(this BitCoinComponent self,
            string account)
        {
            try
            {
                var address = await self.GetNewAddress(account);

                var privateKey = await self.DumpPrivKey(address);

                return (address, privateKey);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return (null, null);
        }

        /// <summary>
        /// 查询钱包余额
        /// </summary>
        /// <param name="self"></param>
        /// <param name="account"></param>
        /// <param name="confirmations"></param>
        /// <param name="watchOnlyIncl"></param>
        /// <returns>返回钱包中指定账户的比特币数量，该调用 需要节点启用钱包功能</returns>
        public static async STask<double> GetBalance(this BitCoinComponent self, string account, int confirmations = 6,
            bool watchOnlyIncl = true)
        {
            var response =
                await HttpClientComponent.Instance.CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<double>>(
                    self.Url, self.JsonId, self.Authentication, "getbalance",
                    account, confirmations, watchOnlyIncl);

            return response.Result;
        }

        /// <summary>
        /// 向指定地址发送比特币
        /// </summary>
        /// <param name="self"></param>
        /// <param name="fromAddress">发送人地址</param>
        /// <param name="toAddress">接收地址</param>
        /// <param name="amount">发送的比特币数量</param>
        /// <param name="comment">备注文本</param>
        /// <param name="commentTo">备注接收人</param>
        /// <param name="autoFeeSubtract">是否自动扣除手续费，默认值：false</param>
        /// <returns>返回-1表示余额不足。</returns>
        public static async STask<string> SendToAddress(this BitCoinComponent self, string fromAddress,
            string toAddress, double amount,
            string comment, string commentTo, bool autoFeeSubtract = false)
        {
            var balance = await self.GetBalance(fromAddress);

            if (balance <= amount)
            {
                return "-1";
            }

            var response =
                await HttpClientComponent.Instance.CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<string>>(
                    self.Url, self.JsonId, self.Authentication, "sendtoaddress",
                    toAddress, amount, comment, commentTo, autoFeeSubtract);

            return response.Result;
        }

        /// <summary>
        /// 查询最近发生的钱包交易
        /// </summary>
        /// <param name="self"></param>
        /// <param name="account">钱包账户名,为空表示查看钱包全部账号</param>
        /// <param name="count">要提取的交易数量，默认值：10</param>
        /// <param name="skip">要跳过的交易数量，默认值：0</param>
        /// <param name="includeWatchOnly">是否包含watch-only地址，默认值：false</param>
        /// <returns></returns>
        public static async STask<List<BitCoinTransactionInfo>> ListTransactions(this BitCoinComponent self,
            string account = null, int count = 10,
            int skip = 0, bool includeWatchOnly = false)
        {
            var response = await HttpClientComponent.Instance
                .CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<List<BitCoinTransactionInfo>>>(
                    self.Url, self.JsonId, self.Authentication, "listtransactions",
                    account, count, skip, includeWatchOnly);

            return response.Result;
        }

        /// <summary>
        /// 根据交易地址获取交易状态
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static async STask<bool> GetTransactionState(this BitCoinComponent self, string hash)
        {
            var bitTransactionInfo = await self.GetTransaction(hash);

            if (bitTransactionInfo == null)
            {
                return false;
            }

            return bitTransactionInfo.Confirmations > 5;
        }

        /// <summary>
        /// 根据交易地址获取交易信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static async STask<BitTransactionInfo> GetTransaction(this BitCoinComponent self, string hash)
        {
            var response = await HttpClientComponent.Instance
                .CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<BitTransactionInfo>>(
                    self.Url, self.JsonId, self.Authentication, "gettransaction",hash);

            return response?.Result;
        }

        /// <summary>
        /// 生成新地址
        /// </summary>
        /// <param name="self"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        private static async STask<string> GetNewAddress(this BitCoinComponent self, string account)
        {
            var response =
                await HttpClientComponent.Instance.CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<string>>(
                    self.Url, self.JsonId, self.Authentication, "getnewaddress", account);

            return response.Result;
        }
        
        /// <summary>
        /// 获取指定哈希的区块
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static async STask<BitCoinBlockInfo> GetBlock(this BitCoinComponent self, string hash)
        {
            var response =
                await HttpClientComponent.Instance.CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<BitCoinBlockInfo>>(
                    self.Url, self.JsonId, self.Authentication, "getblock", hash);
            return response?.Result;
        }

        /// <summary>
        /// 调用返回本地最优链上最后一个区块的哈希
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async STask<string> GetBestBlockHash(this BitCoinComponent self)
        {
            var response =
                await HttpClientComponent.Instance.CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<string>>(
                    self.Url, self.JsonId, self.Authentication, "getbestblockhash");
            return response?.Result;
        }

        /// <summary>
        /// 导出指定私钥
        /// </summary>
        /// <param name="self"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        private static async STask<string> DumpPrivKey(this BitCoinComponent self, string address)
        {
            var response =
                await HttpClientComponent.Instance.CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<string>>(
                    self.Url, self.JsonId, self.Authentication, "dumpprivkey", address);

            return response.Result;
        }
    }
}