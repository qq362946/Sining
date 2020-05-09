// using System;
// using System.Collections.Generic;
// using System.Linq;
// using MongoDB.Driver;
// using Sining.Config;
// using Sining.Model;
// using Sining.Tools;
//
// namespace Sining.Module
// {
//     public static class FinancingComponentSystem
//     {
//         /// <summary>
//         /// 初始化
//         /// </summary>
//         /// <param name="self"></param>
//         /// <returns></returns>
//         public static async STask Init(this FinancingComponent self)
//         {
//             self.DbComponent = await self.GetDbComponent(ConstValue.FinancingDBString, DateTime.Now.Year);
//
//             FinancingComponent.Instance = self;
//         }
//
//         /// <summary>
//         /// 生成一个新的财务实体类
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="userId">用户Id</param>
//         /// <param name="coinId">币类型</param>
//         /// <param name="financingConfigType">资金变动类型ID</param>
//         /// <param name="money">充值金额</param>
//         /// <param name="balance">余额</param>
//         /// <param name="remark">备注</param>
//         /// <returns></returns>
//         public static FinancingInfo Create(this FinancingComponent self, long userId, CoinConfigType coinId,
//             FinancingConfigType financingConfigType, decimal money, decimal balance,
//             string remark = "")
//         {
//             var financingInfo = ComponentFactory.Create<FinancingInfo>(self.Scene);
//
//             financingInfo.UserId = userId;
//             financingInfo.CoinId = (int) coinId;
//             financingInfo.CoinName = CoinConfigData.Instance.GetCoinConfig(coinId).EnName;
//             financingInfo.AddTime = TimeHelper.Now;
//             financingInfo.FinancingConfigType = financingConfigType;
//             financingInfo.MoldTitle = FinancingConfigData.Instance.GetConfig((int) financingConfigType).title;
//             financingInfo.Money = money;
//             financingInfo.Balance = balance;
//             financingInfo.Remark = remark;
//
//             return financingInfo;
//         }
//
//         /// <summary>
//         /// 根据当前数据库获取所有分表充值数据
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="list"></param>
//         /// <returns></returns>
//         public static async STask GetFinancingInfos(this FinancingComponent self,
//             ListComponent<FinancingInfo> list)
//         {
//             for (var fragmentationIndex = 1;
//                 fragmentationIndex <= ConstValue.FinancingFragmentationCount;
//                 fragmentationIndex++)
//             {
//                 var financingInfos = await self.DbComponent.Query<FinancingInfo>(d => true);
//
//                 if (!financingInfos.Any()) continue;
//
//                 list.List.AddRange(financingInfos);
//             }
//         }
//         
//         /// <summary>
//         /// 根据当前数据库获取所有分表充值数据
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="year">年份</param>
//         /// <param name="list"></param>
//         /// <returns></returns>
//         public static async STask GetFinancingInfos(this FinancingComponent self, int year,
//             ListComponent<FinancingInfo> list)
//         {
//             var dbComponent = await self.GetDbComponent(ConstValue.FinancingDBString, year);
//
//             for (var fragmentationIndex = 1;
//                 fragmentationIndex <= ConstValue.FinancingFragmentationCount;
//                 fragmentationIndex++)
//             {
//                 var financingInfos = await dbComponent.Query<FinancingInfo>(d => true);
//
//                 if (!financingInfos.Any()) continue;
//
//                 list.List.AddRange(financingInfos);
//             }
//         }
//         
//         /// <summary>
//         /// 根据用户ID获取用户所有的交易记录
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="id"></param>
//         /// <param name="pageIndex"></param>
//         /// <param name="pageSize"></param>
//         /// <param name="isAsc"></param>
//         /// <returns></returns>
//         public static async STask<List<FinancingInfo>> GetFinancingInfos(this FinancingComponent self,
//             long id, int pageIndex, int pageSize, bool isAsc = true)
//         {
//             return await self.DbComponent.QueryByPageOrderBy<FinancingInfo>(
//                 d => d.UserId == id,
//                 pageIndex,
//                 pageSize,
//                 d => d.AddTime,
//                 isAsc,
//                 self.GetCollectionName(id));
//         }
//
//         /// <summary>
//         /// 根据用户ID获取用户所有的交易记录
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="id"></param>
//         /// <param name="year"></param>
//         /// <param name="pageIndex"></param>
//         /// <param name="pageSize"></param>
//         /// <param name="isAsc"></param>
//         /// <returns></returns>
//         public static async STask<List<FinancingInfo>> GetFinancingInfos(this FinancingComponent self,
//             long id, int year, int pageIndex, int pageSize, bool isAsc = true)
//         {
//             var dbComponent = await self.GetDbComponent(ConstValue.FinancingDBString, year);
//             
//             return await dbComponent.QueryByPageOrderBy<FinancingInfo>(
//                 d => d.UserId == id,
//                 pageIndex,
//                 pageSize,
//                 d => d.AddTime,
//                 isAsc,
//                 self.GetCollectionName(id));
//         }
//         
//         /// <summary>
//         /// 保存充值记录
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="financingInfo"></param>
//         /// <returns></returns>
//         public static async STask SaveFinancingInfo(this FinancingComponent self,
//             FinancingInfo financingInfo)
//         {
//             await self.DbComponent.Save(financingInfo, self.GetCollectionName(financingInfo.UserId));
//         }
//         
//         /// <summary>
//         /// 保存充值账单
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="clientSessionHandle"></param>
//         /// <param name="financingInfo"></param>
//         /// <returns></returns>
//         public static async STask SaveFinancingInfo(this FinancingComponent self,
//             IClientSessionHandle clientSessionHandle, FinancingInfo financingInfo)
//         {
//             await self.DbComponent.Save(clientSessionHandle, financingInfo,
//                 self.GetCollectionName(financingInfo.UserId));
//         }
//         
//         /// <summary>
//         /// 获取数据库连接
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="connectionString"></param>
//         /// <param name="year"></param>
//         /// <param name="addSelf"></param>
//         /// <returns></returns>
//         private static async STask<MongoDBComponent> GetDbComponent(this FinancingComponent self,
//             string connectionString,
//             int year, bool addSelf = true)
//         {
//             var dbName = $"HBYXFinancing{year}";
//
//             MongoDBComponent dbComponent;
//
//             if (addSelf)
//             {
//                 dbComponent = self.AddComponent<MongoDBComponent, string, string>(
//                     connectionString, dbName);
//             }
//             else
//             {
//                 dbComponent = ComponentFactory.CreateOnly<MongoDBComponent, string, string>(
//                     self.Scene, connectionString, dbName);
//             }
//
//             var sss = await dbComponent.MongoDatabase.ListCollectionNamesAsync();
//             
//             // 创建充值分表
//
//             var collectionNames = await (await dbComponent.MongoDatabase.ListCollectionNamesAsync()).ToListAsync();
//
//             for (var i = 1; i <= ConstValue.FinancingFragmentationCount; i++)
//             {
//                 var collectionName = $"{nameof(FinancingInfo)}{i}";
//
//                 if (collectionNames.Contains(collectionName)) continue;
//
//                 await dbComponent.MongoDatabase.CreateCollectionAsync(collectionName);
//             }
//
//             return dbComponent;
//         }
//
//         /// <summary>
//         /// 根据id获得所在的表名称
//         /// </summary>
//         /// <param name="self"></param>
//         /// <param name="id"></param>
//         /// <returns></returns>
//         private static string GetCollectionName(this FinancingComponent self, long id)
//         {
//             return $"{nameof(FinancingInfo)}{id % ConstValue.FinancingFragmentationCount}";
//         }
//     }
// }