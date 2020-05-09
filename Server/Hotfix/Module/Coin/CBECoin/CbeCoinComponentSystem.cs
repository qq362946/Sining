// using System;
// using System.Linq;
// using System.Threading;
// using MongoDB.Driver;
// using Sining.Config;
// using Sining.Model;
// using Sining.Tools;
//
// namespace Sining.Module
// {
//     public static class CbeCoinComponentSystem
//     {
//         public static async STask Init(this CbeCoinComponent self)
//         {
//             self.Clear();
//             
//             // 初始化区块链接口。
//             
//             self.Init("CBE");
//
//             // 获取当前区块链信息。
//             
//             self.CbeConfigComponent =
//                 await SingleDataHeler.Get<CbeConfigComponent>(self.Scene, SingleDataType.CbeConfigData);
//
//             if (self.CbeConfigComponent == null)
//             {
//                 // 如果没有区块链信息，就创建一个新的并保存到数据库中。
//                 
//                 self.CbeConfigComponent = ComponentFactory.CreateOnly<CbeConfigComponent>(self.Scene);
//                 self.CbeConfigComponent.Id = SingleDataType.CbeConfigData;
//
//                 self.CbeConfigComponent.Block = await self.GetBlock();
//
//                 await SingleDataHeler.Save(self.CbeConfigComponent);
//
//                 Log.Info("CbeConfig初始化完成");
//             }
//             else
//             {
//                 self.CbeConfigComponent.Scene = self.Scene;
//             }
//
//             // 获取等待充值确认的记录。
//
//             await self.GetCoinRechargeInfos();
//             
//             // 开始执行充值任务。
//
//             self.RunTask();
//         }
//         /// <summary>
//         /// 获取等待充值确认的记录
//         /// </summary>
//         /// <param name="self"></param>
//         /// <returns></returns>
//         private static async STask GetCoinRechargeInfos(this CbeCoinComponent self)
//         {
//             using var coinRechargeInfos = ObjectPool<ListComponent<CoinRechargeInfo>>.Rent();
//             await CoinRechargeComponent.Instance.GetCoinRechargeInfos(coinRechargeInfos);
//
//             foreach (var coinRechargeInfo in coinRechargeInfos.List)
//             {
//                 self.WaitCoinRechargeInfos[coinRechargeInfo.Hash] = coinRechargeInfo;
//             }
//         }
//         /// <summary>
//         /// 开始执行充值任务
//         /// </summary>
//         /// <param name="self"></param>
//         private static void RunTask(this CbeCoinComponent self)
//         {
//             var instanceId = self.InstanceId;
//
//             // 开启一个线程来执行任务（不要使用线程池）。
//
//             self.CurrentTask = new Thread(async () =>
//             {
//                 while (instanceId == self.InstanceId)
//                 {
//                     try
//                     {
//                         // 获得当前最新块的位置，并计算当前处理的位置的差值
//
//                         var block = await self.GetBlock();
//                         
//                         if (instanceId != self.InstanceId) return;
//
//                         var blockValue = block - self.CbeConfigComponent.Block;
//
//                         if (block > 0 && blockValue > 0)
//                         {
//                             await self.CollectionBlockInfo(blockValue, instanceId);
//                         }
//                     }
//                     catch (Exception e)
//                     {
//                         Log.Error(e);
//                     }
//                     
//                     Log.Debug("CBE充值任务执行完成，下次执行再10000后!");
//                     
//                     Thread.Sleep(10000);
//                 }
//                 
//                 Log.Debug("CBE充值服务器已经停止...");
//                 
//             }) {IsBackground = true};
//
//             self.CurrentTask.Start();
//         }
//         /// <summary>
//         /// 采集区块交易记录
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="blockValue"></param>
//         /// <param name="instanceId"></param>
//         /// <returns></returns>
//         private static async STask CollectionBlockInfo(this CbeCoinComponent self, long blockValue,long instanceId)
//         {
//             var coinConfig = CoinConfigData.Instance.GetCoinConfig(CoinConfigType.CBE);
//
//             // 获取区块交易记录到最新块
//
//             for (var i = 1; i <= blockValue; i++)
//             {
//                 var currentBlock = self.CbeConfigComponent.Block + i;
//
//                 try
//                 {
//                     var transfers = await self.GetTransferByBlock(currentBlock);
//
//                     if (instanceId != self.InstanceId) return;
//                     
//                     if (transfers == null)
//                     {
//                         Log.Info($"CBEBlock:{currentBlock} is null");
//                         return;
//                     }
//                     
//                     var transfersList = transfers.Where(
//                         d => d.Input.Length == 138 &&
//                              d.To == coinConfig.Ext);
//
//                     foreach (var transaction in transfersList)
//                     {
//                         try
//                         {
//                             var inputMemory = transaction.Input.AsMemory();
//
//                             var money = self.HexToDecimal(inputMemory.Slice(74).ToString());
//
//                             if (money <= coinConfig.MinRecharge ||
//                                 self.WaitCoinRechargeInfos.ContainsKey(transaction.TransactionHash))
//                             {
//                                 continue;
//                             }
//
//                             // 创建交易信息实体。
//
//                             var coinRechargeInfo =
//                                 ComponentFactory.Create<CoinRechargeInfo>(self.Scene, eventSystem: false);
//
//                             coinRechargeInfo.Hash = transaction.TransactionHash;
//                             coinRechargeInfo.Block = currentBlock;
//                             coinRechargeInfo.From = transaction.From;
//                             coinRechargeInfo.Address = $"0x{inputMemory.Slice(34, 40)}";
//                             coinRechargeInfo.Money = money;
//
//                             // 根据转账地址找到用户信息
//                             
//                             var user =
//                                 await UserManageComponent.Instance.GetUserByWalletsAddress(coinRechargeInfo.Address);
//                             if (user == null)
//                             {
//                                 Log.Warning(
//                                     $"No corresponding user found Address:{coinRechargeInfo.Address} transaction:{transaction.ToJson()}");
//                                 coinRechargeInfo.Dispose();
//                                 continue;
//                             }
//
//                             coinRechargeInfo.UserId = user.Id;
//                             coinRechargeInfo.AddTime = TimeHelper.Now;
//                             coinRechargeInfo.CoinId = (int) CoinConfigType.CBE;
//                             coinRechargeInfo.CoinName = self.NodeName;
//
//                             // 添加到待保存数据库数组中。
//
//                             self.CoinRechargeInfos.Add(coinRechargeInfo);
//
//                             // 添加到等待充值确认的记录。
//
//                             self.WaitCoinRechargeInfos[coinRechargeInfo.Hash] = coinRechargeInfo;
//                         }
//                         catch (Exception e)
//                         {
//                             Log.Error(
//                                 $"TransactionHash:{transaction.TransactionHash} Exception {e.Message}");
//                         }
//                     }
//                 }
//                 catch (Exception e)
//                 {
//                     Log.Error(e);
//                     return;
//                 }
//             }
//             
//             // 保存充值记录
//
//             if (!await self.SaveCoinRechargeInfos(blockValue, instanceId))
//             {
//                 return;
//             }
//
//             // 确认交易
//             
//             await self.CheckCoinRechargeInfos(instanceId);
//         }
//         /// <summary>
//         /// 保存交易记录
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="blockValue"></param>
//         /// <param name="instanceId"></param>
//         /// <returns></returns>
//         private static async STask<bool> SaveCoinRechargeInfos(this CbeCoinComponent self, long blockValue, long instanceId)
//         {
//             var dbComponent = self.DataBase();
//
//             var currentBlock = self.CbeConfigComponent.Block;
//
//             // 开启一个事务
//
//             using var clientSession = await dbComponent.GetConnection<MongoClient>().StartSessionAsync();
//
//             try
//             {
//                 if (instanceId != self.InstanceId) return false;
//
//                 clientSession.StartTransaction(new TransactionOptions(
//                     readConcern: ReadConcern.Snapshot,
//                     writeConcern: WriteConcern.WMajority));
//
//                 // 插入充值记录
//                 
//                 foreach (var selfCoinRechargeInfo in self.CoinRechargeInfos)
//                 {
//                     await CoinRechargeComponent.Instance.SaveCoinRechargeInfo(clientSession, selfCoinRechargeInfo);
//                 }
//
//                 if (instanceId != self.InstanceId)
//                 {
//                     await clientSession.AbortTransactionAsync();
//                     return false;
//                 }
//
//                 // 保存当前区块位置到数据库
//
//                 self.CbeConfigComponent.Block += blockValue;
//                 await self.CbeConfigComponent.Save(clientSession);
//
//                 // 提交事务
//
//                 await clientSession.CommitTransactionAsync();
//             }
//             catch (Exception e)
//             {
//                 // 回滚事务
//                 
//                 await clientSession.AbortTransactionAsync();
//                 
//                 self.CbeConfigComponent.Block = currentBlock;
//                 
//                 Log.Error(e);
//
//                 return false;
//             }
//             finally
//             {
//                 self.CoinRechargeInfos.Clear();
//             }
//
//             return true;
//         }
//         /// <summary>
//         /// 确认交易
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="instanceId"></param>
//         /// <returns></returns>
//         private static async STask CheckCoinRechargeInfos(this CbeCoinComponent self,long instanceId)
//         {
//             if (self.InstanceId != instanceId || !self.WaitCoinRechargeInfos.Any())
//             {
//                 return;
//             }
//             
//             // 开启一个事务
//             
//             var dbComponent = self.DataBase();
//
//             using var clientSession = await dbComponent.GetConnection<MongoClient>().StartSessionAsync();
//
//             if (instanceId != self.InstanceId) return;
//
//             clientSession.StartTransaction(new TransactionOptions(
//                 readConcern: ReadConcern.Snapshot,
//                 writeConcern: WriteConcern.WMajority));
//
//             try
//             {
//                 //1、修改充值记录。
//                 //2、修改用户余额。
//                 //3、保存财务账单。
//
//                 foreach (var selfWaitCoinRechargeInfo in self.WaitCoinRechargeInfos.Values)
//                 {
//                     if (self.InstanceId != instanceId)
//                     {
//                         return;
//                     }
//
//                     selfWaitCoinRechargeInfo.Confirms =
//                         (int) (self.CbeConfigComponent.Block - selfWaitCoinRechargeInfo.Block + 1);
//
//                     // 如果确认交易数小于2个。不进行处理。
//
//                     if (selfWaitCoinRechargeInfo.Confirms < 2)
//                     {
//                         continue;
//                     }
//
//                     // 保存交易记录
//
//                     selfWaitCoinRechargeInfo.RechargeStatus = true;
//                     await CoinRechargeComponent.Instance.SaveCoinRechargeInfo(clientSession, selfWaitCoinRechargeInfo);
//
//                     // 修改用户余额
//
//                     var user = await UserManageComponent.Instance.GetUser(selfWaitCoinRechargeInfo.UserId);
//
//                     if (self.InstanceId != instanceId) return;
//
//                     var wallet = user?.Wallets?.FirstOrDefault();
//
//                     if (user == null || wallet == null)
//                     {
//                         Log.Warning(
//                             $"UserId:{selfWaitCoinRechargeInfo.UserId} WaitCoinRechargeInfo;{selfWaitCoinRechargeInfo.ToJson()} No corresponding user found");
//
//                         await clientSession.AbortTransactionAsync();
//
//                         continue;
//                     }
//
//                     wallet.Money += selfWaitCoinRechargeInfo.Money;
//                     await dbComponent.Save(clientSession, user);
//
//                     // 保存财务账单
//                     
//                     if (self.InstanceId != instanceId) return;
//
//                     var financingInfo = ComponentFactory.CreateOnly<FinancingInfo>(self.Scene, eventSystem: false);
//
//                     financingInfo.UserId = selfWaitCoinRechargeInfo.UserId;
//                     financingInfo.CoinId = (int) CoinConfigType.CBE;
//                     financingInfo.CoinName = CoinConfigData.Instance.GetConfig(financingInfo.CoinId).Name;
//                     financingInfo.AddTime = TimeHelper.Now;
//                     financingInfo.FinancingConfigType = FinancingConfigType.recharge;
//                     financingInfo.MoldTitle =
//                         FinancingConfigData.Instance.GetConfig((int) FinancingConfigType.recharge).title;
//                     financingInfo.Money = selfWaitCoinRechargeInfo.Money;
//                     financingInfo.Balance = wallet.Money;
//
//                     await FinancingComponent.Instance.SaveFinancingInfo(clientSession, financingInfo);
//                 }
//                 
//                 if (self.InstanceId != instanceId) return;
//
//                 await clientSession.CommitTransactionAsync();
//                 
//                 // 清除待处理充值数据
//                 
//                 foreach (var coinRechargeInfo in self.WaitCoinRechargeInfos.Values)
//                 {
//                     coinRechargeInfo.Dispose();
//                 }
//                 
//                 self.WaitCoinRechargeInfos.Clear();
//             }
//             catch (Exception e)
//             {
//                 await clientSession.AbortTransactionAsync();
//                 
//                 Log.Error(e);
//             }
//         }
//     }
// }