// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using MongoDB.Driver;
// using Sining.Config;
// using Sining.Model;
// using Sining.Tools;
//
// namespace Sining.Module
// {
//     public static class WithdrawManageComponentSystem
//     {
//         /// <summary>
//         /// 初始化
//         /// </summary>
//         /// <param name="self"></param>
//         /// <returns></returns>
//         public static async STask Init(this WithdrawManageComponent self)
//         {
//             // 加载没有审核、审核过但没有处理的提现信息
//
//             var withdrawInfos = await self.DataBase().Query<WithdrawInfo>(d => d.Status == 0 || d.Status == 1);
//             
//             self.WaitWithdrawInfos.Clear();
//             self.WithdrawInfoDic.Clear();
//             
//             foreach (var withdrawInfo in withdrawInfos)
//             {
//                 if (withdrawInfo.Status == 1)
//                 {
//                     self.WithdrawInfoDic.TryAdd(withdrawInfo.Id, withdrawInfo);
//                     continue;
//                 }
//                 
//                 self.WaitWithdrawInfos.Add(withdrawInfo.UserId, withdrawInfo);
//                 self.WithdrawInfos.Add(withdrawInfo.Id, withdrawInfo);
//             }
//
//             // self.RunTask();
//             
//             WithdrawManageComponent.Instance = self;
//         }
//
//         /// <summary>
//         /// 提现定时服务
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="time"></param>
//         private static void RunTask(this WithdrawManageComponent self, int time = 20000)
//         {
//             var instanceId = self.InstanceId;
//
//             self.CurrentTask = new Thread(async () =>
//             {
//                 Log.Debug($"提现处理服务已经启动，下次处理时间:{time}毫秒");
//
//                 while (instanceId == self.InstanceId)
//                 {
//                     try
//                     {
//                         // 所有已经审核过但没有处理过的提现单。
//
//                         var withdrawInfos = self.WithdrawInfoDic.Values.ToList();
//
//                         foreach (var withdrawInfo in withdrawInfos)
//                         {
//                             // 开启一个事务
//
//                             using var clientSession =
//                                 await self.DataBase().GetConnection<MongoClient>().StartSessionAsync();
//
//                             FinancingInfo financingInfo = null;
//
//                             try
//                             {
//                                 // 区块链上进行转账操作。
//
//                                 if (!await self.Transfer(withdrawInfo)) continue;
//
//                                 if (instanceId != self.InstanceId) return;
//
//                                 clientSession.StartTransaction(new TransactionOptions(
//                                     readConcern: ReadConcern.Snapshot,
//                                     writeConcern: WriteConcern.WMajority));
//
//                                 // 提现金额到用户中，并保存到数据库。
//
//                                 var ( result, balance) = await UserManageComponent.Instance.SetWithdrawMoney(
//                                     clientSession,
//                                     withdrawInfo.UserId,
//                                     withdrawInfo.CoinId,
//                                     withdrawInfo.Real,
//                                     withdrawInfo.FeeCoin,
//                                     withdrawInfo.Fee);
//
//                                 if (instanceId != self.InstanceId)
//                                 {
//                                     await clientSession.AbortTransactionAsync();
//                                     return;
//                                 }
//
//                                 if (!result) continue;
//
//                                 // 添加一条财务记录。
//
//                                 financingInfo = FinancingComponent.Instance.Create(
//                                     withdrawInfo.UserId,
//                                     withdrawInfo.CoinId,
//                                     FinancingConfigType.withdraw,
//                                     withdrawInfo.Real,
//                                     balance);
//
//                                 await FinancingComponent.Instance.SaveFinancingInfo(clientSession, financingInfo);
//
//                                 if (instanceId != self.InstanceId)
//                                 {
//                                     await clientSession.AbortTransactionAsync();
//                                     financingInfo.Dispose();
//                                     return;
//                                 }
//
//                                 withdrawInfo.Status = 2;
//
//                                 // 保存提现记录到数据库。
//
//                                 await self.DataBase().Save(clientSession, withdrawInfo);
//
//                                 await clientSession.CommitTransactionAsync();
//
//                                 self.WithdrawInfoDic.Remove(withdrawInfo.Id, out var removeWithdrawInfo);
//
//                                 Log.Info($"处理了一个新的提现单子:{removeWithdrawInfo.ToJson()}");
//
//                                 withdrawInfo.Dispose();
//                                 financingInfo.Dispose();
//                             }
//                             catch (Exception e)
//                             {
//                                 // 如果发生错误就要回滚到没有处理之前的状态。
//
//                                 Log.Error(e);
//
//                                 withdrawInfo.Hash = "";
//                                 withdrawInfo.ProcessTime = 0;
//
//                                 financingInfo?.Dispose();
//
//                                 await clientSession.AbortTransactionAsync();
//                             }
//                         }
//                     }
//                     catch (Exception e)
//                     {
//                         Log.Error(e);
//                     }
//
//                     Thread.Sleep(time);
//                 }
//             }) {IsBackground = true};
//
//             self.CurrentTask.Start();
//         }
//
//         /// <summary>
//         /// 创建一个新的提现实体
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="userId">用户ID</param>
//         /// <param name="address">提现地址</param>
//         /// <param name="money">提现金额</param>
//         /// <param name="feeCoin">手续费币种</param>
//         /// <param name="processMold">处理模式，0区块处理，1外部处理</param>
//         /// <param name="status">状态，-1,驳回,0待处理,1处理中,2已处理</param>
//         /// <param name="coinId">提现币种</param>
//         /// <param name="remark">提现备注</param>
//         /// <returns></returns>
//         public static WithdrawInfo Create(this WithdrawManageComponent self,
//             long userId,
//             string address,
//             decimal money,
//             CoinConfigType feeCoin,
//             int processMold,
//             int status,
//             CoinConfigType coinId,
//             string remark = "")
//         {
//             var withdrawInfo = ComponentFactory.Create<WithdrawInfo>(self.Scene);
//
//             withdrawInfo.UserId = userId;
//             withdrawInfo.Address = address;
//             withdrawInfo.Money = money;
//             withdrawInfo.FeeCoin = feeCoin;
//
//             var coinDate = CoinConfigData.Instance.GetCoinConfig(feeCoin);
//             
//             withdrawInfo.FeeCoinEname = coinDate.EnName;
//             withdrawInfo.Fee = coinDate.MinWithDrawFee;
//             
//             withdrawInfo.Real = money - withdrawInfo.Fee;
//             withdrawInfo.ProcessMold = processMold;
//             withdrawInfo.Status = status;
//             withdrawInfo.AddTime = TimeHelper.Now;
//             withdrawInfo.CoinId = coinId;
//             withdrawInfo.CoinName = CoinConfigData.Instance.GetCoinConfig(coinId).EnName;
//             withdrawInfo.Remark = remark;
//
//             return withdrawInfo;
//         }
//
//         /// <summary>
//         /// 添加一个提现账单
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="withdrawInfo"></param>
//         /// <returns></returns>
//         public static async STask<string> AddWithdraw(this WithdrawManageComponent self, WithdrawInfo withdrawInfo)
//         {
//             // 提现的金额不能小于0。
//
//             if (withdrawInfo.Money < 0)
//             {
//                 return ErrorCode.AddWithdrawMoneyLessZero;
//             }
//
//             // 不能小于配表里的最低提现金额。
//
//             if (withdrawInfo.Money < CoinConfigData.Instance.GetCoinConfig(withdrawInfo.CoinId).MinWithDraw)
//             {
//                 return ErrorCode.AddWithdrawMinWithDraw;
//             }
//
//             // 根据提现的UserId来查找用户信息。
//
//             var user = await UserManageComponent.Instance.GetUser(withdrawInfo.UserId);
//
//             if (user == null)
//             {
//                 return ErrorCode.AddWithdrawNotFoundUser;
//             }
//
//             // 根据手续费币种类型获得对应的钱包。
//
//             var feeWallet = user.GetWallet(withdrawInfo.FeeCoin);
//
//             if (feeWallet == null)
//             {
//                 return ErrorCode.AddWithdrawNotFoundFeeWallet;
//             }
//
//             var feeCoinConfig = CoinConfigData.Instance.GetCoinConfig(withdrawInfo.FeeCoin);
//
//             // 手续费不能小于配表的最低提现费用。
//
//             if (feeWallet.Money < feeCoinConfig.MinWithDrawFee)
//             {
//                 return ErrorCode.AddWithdrawMinWithDrawFee;
//             }
//
//             feeWallet.Money -= feeCoinConfig.MinWithDrawFee;
//             feeWallet.Forzen = feeCoinConfig.MinWithDrawFee;
//
//             try
//             {
//                 // 保存用户数据和提现订单数据。
//
//                 await self.DataBase().Save(withdrawInfo);
//                 await UserManageComponent.Instance.Save(user);
//                 
//                 // 添加到缓存中，方便下次读取。
//
//                 self.WaitWithdrawInfos.Add(withdrawInfo.UserId, withdrawInfo);
//                 self.WithdrawInfos.Add(withdrawInfo.Id, withdrawInfo);
//             }
//             catch (Exception e)
//             {
//                 Log.Error(e);
//             }
//
//             return null;
//         }
//
//         /// <summary>
//         /// 获取待处理提现订单
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="pageIndex"></param>
//         /// <param name="pageSize"></param>
//         /// <returns></returns>
//         public static List<WithdrawInfo> GetWaitWithdrawInfos(this WithdrawManageComponent self,
//             int pageIndex, int pageSize)
//         {
//             return self.WithdrawInfos.Values.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
//         }
//
//         /// <summary>
//         /// 根据用户ID查找提现记录
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="id"></param>
//         /// <param name="pageIndex"></param>
//         /// <param name="pageSize"></param>
//         /// <returns></returns>
//         public static List<WithdrawInfo> GetWaitWithdrawInfosByUserId(this WithdrawManageComponent self,long id, int pageIndex,
//             int pageSize)
//         {
//             return !self.WaitWithdrawInfos.TryGetValue(id, out var list) ? list : list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
//         }
//
//         /// <summary>
//         /// 审核提现记录
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="id"></param>
//         /// <returns></returns>
//         public static async STask<bool> Approval(this WithdrawManageComponent self, long id)
//         {
//             if (!self.WithdrawInfos.TryGetValue(id, out var withdrawInfo))
//             {
//                 return false;
//             }
//
//             // 修改记录状态为已经处理，并保存到数据库。
//
//             withdrawInfo.Status = 1;
//             await self.DataBase().Save(withdrawInfo);
//             self.WithdrawInfos.Remove(id);
//             
//             // 添加到待处理提现账单
//
//             self.WithdrawInfoDic.TryAdd(withdrawInfo.Id, withdrawInfo);
//
//             return true;
//         }
//
//         /// <summary>
//         /// 区块链上进行转账
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="withdrawInfo">提现记录信息</param>
//         /// <returns></returns>
//         private static async STask<bool> Transfer(this WithdrawManageComponent self, WithdrawInfo withdrawInfo)
//         {
//             var coinConfig = CoinConfigData.Instance.GetCoinConfig(withdrawInfo.CoinId);
//             
//             var hash = "";
//             
//             switch (withdrawInfo.CoinId)
//             {
//                 case CoinConfigType.USDT:
//                     break;
//                 case CoinConfigType.CBE:
//                 {
//                     var coinComponent = self.Scene.GetComponent<CbeCoinComponent>();
//                     
//                     hash = await coinComponent.SendContractTransaction(
//                         coinConfig.MainAddress,
//                         "123456",
//                         withdrawInfo.Address, withdrawInfo.Real);
//                     
//                     if (hash == "-2" || 
//                         hash == ErrorCode.MoacConnectionFailed || 
//                         hash == ErrorCode.MoacInsufficientBalance ||
//                         hash == ErrorCode.MoacOtherError) return false;
//
//                     break;
//                 }
//                 case CoinConfigType.FST:
//                     break;
//                 case CoinConfigType.FIL:
//                     break;
//                 case CoinConfigType.ETH:
//                     break;
//                 case CoinConfigType.COA:
//                     break;
//                 default:
//                     throw new Exception($"not found CoinConfigType By {withdrawInfo.CoinId}");
//             }
//             
//             withdrawInfo.Hash = hash;
//             withdrawInfo.ProcessTime = TimeHelper.Now;
//
//             return true;
//         }
//     }
// }