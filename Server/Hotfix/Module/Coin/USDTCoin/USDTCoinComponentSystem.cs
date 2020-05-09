using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Sining.Event;
using Sining.Network;

namespace Sining.Module
{
    [ComponentSystem]
    public class USDTCoinComponentAwakeSystem : AwakeSystem<USDTCoinComponent, string, string, string, string>
    {
        protected override void Awake(USDTCoinComponent self, string url, string user, string passWord, string nodeName)
        {
            self.Url = url;
            self.NodeName = nodeName;
            self.Authentication = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{user}:{passWord}")
                ));
        }
    }
    
    public static class USDTCoinComponentSystem
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="self"></param>
        /// <param name="url"></param>
        /// <param name="jsonId"></param>
        /// <param name="nodeName"></param>
        public static void Init(this USDTCoinComponent self, string url, int jsonId, string nodeName)
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
        public static void Init(this USDTCoinComponent self, string url, int jsonId, string user, string passWord,
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
        public static async STask<(string address, string privateKey)> CreateAccount(this USDTCoinComponent self,
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
        /// <returns>返回钱包中指定账户的USDT币数量，该调用 需要节点启用钱包功能</returns>
        public static async STask<double> GetBalance(this USDTCoinComponent self, string account)
        {
            var response =
                await HttpClientComponent.Instance.CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<UsdtBalanceInfo>>(
                    self.Url, self.JsonId, self.Authentication, "omni_getbalance",
                    account, 1);

            return double.Parse(response.Result.Balance);
        }

        /// <summary>
        /// 向指定地址发送比特币
        /// </summary>
        /// <param name="self"></param>
        /// <param name="fromAddress">发送人地址</param>
        /// <param name="toAddress">接收地址</param>
        /// <param name="amount">发送的比特币数量</param>
        /// <returns>返回-1表示余额不足。</returns>
        public static async STask<string> SendToAddress(this USDTCoinComponent self, string fromAddress,
            string toAddress, double amount)
        {
            var balance = await self.GetBalance(fromAddress);

            if (balance <= amount)
            {
                return "-1";
            }

            var response =
                await HttpClientComponent.Instance.CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<string>>(
                    self.Url, self.JsonId, self.Authentication, "omni_send",
                    fromAddress, toAddress, 1, amount.ToString(CultureInfo.InvariantCulture));

            return response.Result;
        }

        /// <summary>
        /// 根据交易hash地址获取交易状态
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static async STask<bool> GetTransactionState(this BitCoinComponent self, string hash)
        {
            var usdtTransactionInfo = await self.GetTransaction(hash);

            if (usdtTransactionInfo == null)
            {
                return false;
            }

            return usdtTransactionInfo.Confirmations > 5;
        }

        /// <summary>
        /// 根据交易hash地址获取交易记录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static async STask<UsdtTransactionInfo> GetTransaction(this USDTCoinComponent self, string hash)
        {
            var result = await HttpClientComponent.Instance
                .CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<UsdtTransactionInfo>>(
                    self.Url, self.JsonId, self.Authentication, "omni_gettransaction",
                    hash);

            return result?.Result;
        }

        /// <summary>
        /// 生成新地址
        /// </summary>
        /// <param name="self"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        private static async STask<string> GetNewAddress(this USDTCoinComponent self, string account)
        {
            var response =
                await HttpClientComponent.Instance.CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<string>>(
                    self.Url, self.JsonId, self.Authentication, "getnewaddress", account);

            return response.Result;
        }
        
        /// <summary>
        /// 导出指定私钥
        /// </summary>
        /// <param name="self"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        private static async STask<string> DumpPrivKey(this USDTCoinComponent self, string address)
        {
            var response =
                await HttpClientComponent.Instance.CallJsonRpc<BitCoinJsonRpcRequest, BitCoinJsonRpcResponse<string>>(
                    self.Url, self.JsonId, self.Authentication, "dumpprivkey", address);

            return response.Result;
        }
    }
}