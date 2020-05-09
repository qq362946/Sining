using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Sining.Module
{
    public static class CoinRechargeComponentSystem
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async STask Init(this CoinRechargeComponent self)
        {
            self.DbComponent = await self.GetDbComponent(ConstValue.CoinRechargeDBString, DateTime.Now.Year);

            CoinRechargeComponent.Instance = self;
        }
        
        /// <summary>
        /// 根据当前数据库获取所有分表充值数据
        /// </summary>
        /// <param name="self"></param>
        /// <param name="list"></param>
        /// <param name="getAll">false 只获取待处理的充值数据 true 获取全部数据</param>
        /// <returns></returns>
        public static async STask GetCoinRechargeInfos(this CoinRechargeComponent self,
            ListComponent<CoinRechargeInfo> list, bool getAll = false)
        {
            for (var fragmentationIndex = 1;
                fragmentationIndex <= ConstValue.CoinRechargeFragmentationCount;
                fragmentationIndex++)
            {
                var coinRechargeInfos = await (getAll
                    ? self.DbComponent.Query<CoinRechargeInfo>(d => true)
                    : self.DbComponent.Query<CoinRechargeInfo>(d => !d.RechargeStatus));

                if (!coinRechargeInfos.Any()) continue;

                list.List.AddRange(coinRechargeInfos);
            }
        }

        /// <summary>
        /// 根据当前数据库获取所有分表充值数据
        /// </summary>
        /// <param name="self"></param>
        /// <param name="year">年份</param>
        /// <param name="list"></param>
        /// <param name="getAll">false 只获取待处理的充值数据 true 获取全部数据</param>
        /// <returns></returns>
        public static async STask GetCoinRechargeInfos(this CoinRechargeComponent self, int year,
            ListComponent<CoinRechargeInfo> list, bool getAll = false)
        {
            var dbComponent = await self.GetDbComponent(ConstValue.CoinRechargeDBString, year);

            for (var fragmentationIndex = 1;
                fragmentationIndex <= ConstValue.CoinRechargeFragmentationCount;
                fragmentationIndex++)
            {
                var coinRechargeInfos = await (getAll
                    ? dbComponent.Query<CoinRechargeInfo>(d => true)
                    : dbComponent.Query<CoinRechargeInfo>(d => !d.RechargeStatus));

                if (!coinRechargeInfos.Any()) continue;

                list.List.AddRange(coinRechargeInfos);
            }
        }

        /// <summary>
        /// 保存充值记录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="coinRechargeInfo"></param>
        /// <returns></returns>
        public static async STask SaveCoinRechargeInfo(this CoinRechargeComponent self,
            CoinRechargeInfo coinRechargeInfo)
        {
            await self.DbComponent.Save(coinRechargeInfo, self.GetCollectionName(coinRechargeInfo.UserId));
        }

        /// <summary>
        /// 保存充值记录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="clientSessionHandle"></param>
        /// <param name="coinRechargeInfo"></param>
        /// <returns></returns>
        public static async STask SaveCoinRechargeInfo(this CoinRechargeComponent self,
            IClientSessionHandle clientSessionHandle, CoinRechargeInfo coinRechargeInfo)
        {
            await self.DbComponent.Save(clientSessionHandle, coinRechargeInfo,
                self.GetCollectionName(coinRechargeInfo.UserId));
        }

        /// <summary>
        /// 根据用户ID获取用户所有的交易记录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public static async STask<List<CoinRechargeInfo>> GetCoinRechargeInfos(this CoinRechargeComponent self,
            long id, int pageIndex, int pageSize, bool isAsc = true)
        {
            return await self.DbComponent.QueryByPageOrderBy<CoinRechargeInfo>(
                d => d.UserId == id,
                pageIndex,
                pageSize,
                d => d.AddTime,
                isAsc,
                self.GetCollectionName(id));
        }

        /// <summary>
        /// 根据用户ID获取用户所有的交易记录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <param name="year"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public static async STask<List<CoinRechargeInfo>> GetCoinRechargeInfos(this CoinRechargeComponent self,
            long id, int year, int pageIndex, int pageSize, bool isAsc = true)
        {
            var dbComponent = await self.GetDbComponent(ConstValue.CoinRechargeDBString, year);
            
            return await dbComponent.QueryByPageOrderBy<CoinRechargeInfo>(
                d => d.UserId == id,
                pageIndex,
                pageSize,
                d => d.AddTime,
                isAsc,
                self.GetCollectionName(id));
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="self"></param>
        /// <param name="connectionString"></param>
        /// <param name="year"></param>
        /// <param name="addSelf"></param>
        /// <returns></returns>
        private static async STask<MongoDBComponent> GetDbComponent(this CoinRechargeComponent self,
            string connectionString,
            int year, bool addSelf = true)
        {
            var dbName = $"HBYXCoinRecharge{year}";

            MongoDBComponent dbComponent;

            if (addSelf)
            {
                dbComponent = self.AddComponent<MongoDBComponent, string, string>(
                    connectionString, dbName);
            }
            else
            {
                dbComponent = ComponentFactory.CreateOnly<MongoDBComponent, string, string>(
                    self.Scene, connectionString, dbName);
            }
            
            // 创建充值分表

            var collectionNames = await (await dbComponent.MongoDatabase.ListCollectionNamesAsync()).ToListAsync();

            for (var i = 1; i <= ConstValue.CoinRechargeFragmentationCount; i++)
            {
                var collectionName = $"{nameof(CoinRechargeInfo)}{i}";

                if (collectionNames.Contains(collectionName)) continue;

                await dbComponent.MongoDatabase.CreateCollectionAsync(collectionName);
            }

            return dbComponent;
        }

        /// <summary>
        /// 根据id获得所在的表名称
        /// </summary>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string GetCollectionName(this CoinRechargeComponent self, long id)
        {
            return $"{nameof(CoinRechargeInfo)}{id % ConstValue.CoinRechargeFragmentationCount}";
        }
    }
}