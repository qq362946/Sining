using System;
using System.Collections.Generic;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors;
using Nethereum.JsonRpc.Client;
using Sining.Event;
using Nethereum.Web3;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Sining.Config;
using Sining.Tools;

namespace Sining.Module
{
    public static class MoacCoinComponentSystem
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="self"></param>
        /// <param name="nodeName"></param>
        public static void Init(this MoacCoinComponent self, string nodeName)
        {
            var coinConfig = CoinConfigData.Instance.GetCoinConfig(CoinConfigType.CBE);

            self.Init(coinConfig.RPCServer, 101, coinConfig.Ext, "CBE");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="self"></param>
        /// <param name="url"></param>
        /// <param name="jsonId"></param>
        /// <param name="contractAddress"></param>
        /// <param name="nodeName"></param>
        public static void Init(this MoacCoinComponent self, string url, int jsonId, string contractAddress,
            string nodeName)
        {
            self.Web3 = new Web3(url);
            self.Url = url;
            self.NodeName = nodeName;
            self.ContractAddress = contractAddress;
            self.JsonId = jsonId;
        }

        /// <summary>
        /// 创建一个新的账号
        /// </summary>
        /// <param name="self"></param>
        /// <param name="passWord">账号密码</param>
        /// <returns></returns>
        public static async STask<string> CreateAccount(this MoacCoinComponent self,
            string passWord = "123456")
        {
            return await self.Web3.Personal.NewAccount.SendRequestAsync(passWord, self.JsonId);
        }

        /// <summary>
        /// 解锁账号
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountAddress">账号地址</param>
        /// <param name="passWord">账号密码</param>
        /// <returns></returns>
        public static async STask<bool> UnlockAccount(this MoacCoinComponent self, string accountAddress,
            string passWord)
        {
            return await self.Web3.Personal.UnlockAccount.SendRequestAsync(accountAddress, passWord, 30000, self.JsonId);
        }

        /// <summary>
        /// 锁定账号
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountAddress">账号地址</param>
        /// <returns></returns>
        public static async STask<bool> LockAccount(this MoacCoinComponent self, string accountAddress)
        {
            return await self.Web3.Personal.LockAccount.SendRequestAsync(accountAddress, self.JsonId);
        }

        /// <summary>
        /// 获取账户余额
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountAddress">账号地址</param>
        /// <param name="accuracy"></param>
        /// <returns>返回-1表示查询失败。</returns>
        public static async STask<decimal> GetBalance(this MoacCoinComponent self, string accountAddress, int accuracy)
        {
            try
            {
                var balance =
                    await self.Web3.Client.SendRequestAsync<string>(new RpcRequest(self.JsonId, "mc_getBalance", accountAddress,
                        "latest"));

                return self.HexToDecimal(balance, accuracy);
            }
            catch (Exception e)
            {
                Log.Error($"fromAddress:{accountAddress} GetBalance Error {e.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 获取智能合约里账户余额
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountAddress"></param>
        /// <param name="accuracy"></param>
        /// <returns>如果为-1，表示查询失败，具体错误可以查看LOG</returns>
        public static async STask<decimal> BalanceOf(this MoacCoinComponent self, string accountAddress,
            int accuracy = 18)
        {
            BalanceOfFunction balanceOfFunction = null;

            try
            {
                balanceOfFunction = ObjectPool<BalanceOfFunction>.Rent();
                balanceOfFunction.Init(accountAddress);

                var balanceHandler = self.Web3.Eth.GetContractQueryHandler<BalanceOfFunction>();

                var result = await balanceHandler.QueryAsync<BigInteger>(
                    self.ContractAddress,
                    balanceOfFunction);

                if (result.Equals(null))
                {
                    return -1;
                }

                return Web3.Convert.FromWei(result, accuracy);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                ObjectPool<BalanceOfFunction>.Return(balanceOfFunction);
            }

            return -1;
        }

        /// <summary>
        /// 墨客比交易
        /// </summary>
        /// <param name="self"></param>
        /// <param name="fromAddress"></param>
        /// <param name="formPassWord"></param>
        /// <param name="toAddress"></param>
        /// <param name="value"></param>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public static async STask<string> SendTransaction(this MoacCoinComponent self, string fromAddress,
            string formPassWord,
            string toAddress, decimal value, int accuracy = 18)
        {
            try
            {
                if (await self.UnlockAccount(fromAddress, formPassWord))
                {
                    var fromAddressBalance = await self.GetBalance(fromAddress, accuracy);

                    if (fromAddressBalance == -1)
                    {
                        return ErrorCode.MoacConnectionFailed;
                    }

                    if (fromAddressBalance <= value)
                    {
                        return ErrorCode.MoacInsufficientBalance;
                    }

                    var @params = new
                    {
                        from = fromAddress,
                        to = toAddress,
                        gas = "0x76c0",
                        gasPrice = "0x4a817c800",
                        value = self.BigConvertor.ConvertToHex(Web3.Convert.ToWei(value, accuracy)),
                        data = "0x"
                    };

                    return await self.Web3.Client.SendRequestAsync<string>(new RpcRequest(self.JsonId,
                        "mc_sendTransaction",
                        @params));
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return ErrorCode.MoacOtherError;
        }

        /// <summary>
        /// 发送合约交易
        /// </summary>
        /// <param name="self"></param>
        /// <param name="fromAddress"></param>
        /// <param name="formPassWord"></param>
        /// <param name="toAddress"></param>
        /// <param name="value"></param>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public static async STask<string> SendContractTransaction(this MoacCoinComponent self, string fromAddress,
            string formPassWord,
            string toAddress, decimal value, int accuracy = 18)
        {
            try
            {
                // 查询代币余额

                var balanceOf = await self.BalanceOf(fromAddress, accuracy);
                if (balanceOf <= value)
                {
                    return ErrorCode.MoacInsufficientBalance;
                }

                // 解锁账号，并开始转账

                if (await self.UnlockAccount(fromAddress, formPassWord))
                {
                    var valueHex = self.DecimalToHex(value, accuracy).Replace("0x", "")
                        .PadLeft(64, '0').ToLower();

                    var @params = new
                    {
                        from = fromAddress,
                        to = self.ContractAddress,
                        gas = "0x7A120",
                        gasPrice = "0x4A817C800",
                        value = "0x0",
                        data = $"0xa9059cbb000000000000000000000000{toAddress.Substring(2)}{valueHex}"
                    };

                    return await self.Web3.Client.SendRequestAsync<string>(
                        new RpcRequest(self.JsonId, "mc_sendTransaction",
                            @params));
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return "-2";
        }

        /// <summary>
        /// 根据交易hash地址获取交易的状态
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hashAddress"></param>
        /// <returns></returns>
        public static async STask<bool> GetTransferStatusByHash(this MoacCoinComponent self, string hashAddress)
        {
            var transactionReceipt = await self.GetTransferByHash(hashAddress);

            if (transactionReceipt == null || transactionReceipt.Status == null)
            {
                return false;
            }

            return transactionReceipt.Status.Value.IsOne;
        }
        
        /// <summary>
        /// 根据交易hash地址获取交易记录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hashAddress"></param>
        /// <returns>返回null表示没有找到交易</returns>
        public static async STask<TransactionReceipt> GetTransferByHash(this MoacCoinComponent self, string hashAddress)
        {
            try
            {
                return await self.Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(hashAddress);
            }
            catch (Exception e)
            {
                Log.Error(e);

                return null;
            }
        }

        /// <summary>
        /// 获取指定块的所有交易记录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static async STask<Transaction[]> GetTransferByBlock(this MoacCoinComponent self, long block)
        {
            var transactions =
                await self.Web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(block));
           
            return transactions?.Transactions;
        }

        /// <summary>
        /// 获取当前最新块
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async STask<long> GetBlock(this MoacCoinComponent self)
        {
            try
            {
                var blockNumber = await self.Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

                return Convert.ToInt64(blockNumber.Value.ToString());
            }
            catch (Exception e)
            {
                Log.Error($"Could not get blockNumberHex {e}");
                return -1;
            }
        }

        /// <summary>
        /// 根据十六进制转换到值
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hex">十六进制字符串</param>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public static decimal HexToDecimal(this MoacCoinComponent self, string hex, int accuracy = 18)
        {
            return Web3.Convert.FromWei(self.BigConvertor.ConvertFromHex(hex), accuracy);
        }

        /// <summary>
        /// 值转换到十六进制字符串
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value">数值</param>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public static string DecimalToHex(this MoacCoinComponent self, decimal value, int accuracy = 18)
        {
            return self.BigConvertor.ConvertToHex(
                Web3.Convert.ToWei(value, accuracy));
        }
    }
}