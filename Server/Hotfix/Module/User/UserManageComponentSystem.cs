using System.Linq;
using MongoDB.Driver;
using Sining.Message;
using Sining.Tools;

namespace Sining.Module
{
    public static class UserManageComponentSystem
    {
        /// <summary>
        /// 创建一个账号
        /// </summary>
        /// <param name="self"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async STask CreateUser(this UserManageComponent self, User user)
        {
            // CBE钱包
            //var cbeAddress = await self.Scene.GetComponent<CbeCoinComponent>().CreateAccount();
            var userCbeWallet =
                ComponentFactory.Create<UserWallet, CoinConfigType, string, decimal>(self.Scene, CoinConfigType.CBE,
                    "", 0);
            userCbeWallet.Id = user.Id;
            user.Wallets.Add(userCbeWallet);

            await self.DataBase().Save(user);
        }
        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="self"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async STask Save(this UserManageComponent self, User user)
        {
            await self.DataBase().Save(user);
        }
        /// <summary>
        /// 检查邮箱地址是否存在
        /// </summary>
        /// <param name="self"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static async STask<bool> ExistByEmail(this UserManageComponent self, string email)
        {
            return await self.DataBase().Exist<User>(d => d.Email == email);
        }
        /// <summary>
        /// 检查手机号是否存在
        /// </summary>
        /// <param name="self"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static async STask<bool> ExistByMobile(this UserManageComponent self, string mobile)
        {
            return await self.DataBase().Exist<User>(d => d.Mobile == mobile);
        }
        /// <summary>
        /// 查询一个账号
        /// </summary>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async STask<User> GetUser(this UserManageComponent self, long id)
        {
            return await self.DataBase().Query<User>(id);
        }
        /// <summary>
        /// 查询一个账号
        /// </summary>
        /// <param name="self"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async STask<User> GetUserByEmail(this UserManageComponent self, string email, string password)
        {
            return await self.DataBase().First<User>(d => d.Email == email && d.Password == password);
        }
        /// <summary>
        /// 查询一个账号
        /// </summary>
        /// <param name="self"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async STask<User> GetUserByMobile(this UserManageComponent self, string mobile, string password)
        {
            return await self.DataBase().First<User>(d => d.Mobile == mobile && d.Password == password);
        }
        /// <summary>
        /// 根据钱包地址获取账号
        /// </summary>
        /// <param name="self"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static async STask<User> GetUserByWalletsAddress(this UserManageComponent self, string address)
        {
            var list = await self.DataBase().QueryJson<User>($"{{'Wallets.Address':'{address}'}}");

            if (list == null || !list.Any())
            {
                return null;
            }

            return list.FirstOrDefault();
        }
        /// <summary>
        /// 提现金额
        /// </summary>
        /// <param name="self"></param>
        /// <param name="clientSessionHandle"></param>
        /// <param name="userId"></param>
        /// <param name="withdrawCoinType"></param>
        /// <param name="withdrawMoney"></param>
        /// <param name="deductionCoinType"></param>
        /// <param name="deductionMoney"></param>
        /// <returns></returns>
        public static async STask<(bool, decimal)> SetWithdrawMoney(
            this UserManageComponent self,
            IClientSessionHandle clientSessionHandle,
            long userId,
            CoinConfigType withdrawCoinType,
            decimal withdrawMoney,
            CoinConfigType deductionCoinType,
            decimal deductionMoney)
        {
            var user = await self.GetUser(userId);

            if (user == null) return (false, 0);

            decimal userWithdrawMoney = 0;

            foreach (var userWallet in user.Wallets)
            {
                if (userWallet.CoinConfigType == withdrawCoinType)
                {
                    userWallet.Money -= withdrawMoney;
                    userWithdrawMoney = userWallet.Money;
                }
                else if (userWallet.CoinConfigType == deductionCoinType)
                {
                    userWallet.Money -= deductionMoney;
                }
            }

            await self.DataBase().Save(clientSessionHandle, user);

            return (true, userWithdrawMoney);
        }
    }
}