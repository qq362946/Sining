namespace Sining.Module
{
    public static class UserSystem
    {
        /// <summary>
        /// 根据钱包类型获得钱包
        /// </summary>
        /// <param name="self"></param>
        /// <param name="coinConfigType"></param>
        /// <returns></returns>
        public static UserWallet GetWallet(this User self, CoinConfigType coinConfigType)
        {
            foreach (var selfWallet in self.Wallets)
            {
                if (selfWallet.CoinConfigType == coinConfigType)
                {
                    return selfWallet;
                }
            }

            return null;
        }

        /// <summary>
        /// 根据钱包地址获得钱包
        /// </summary>
        /// <param name="self"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static UserWallet GetWallet(this User self, string address)
        {
            foreach (var selfWallet in self.Wallets)
            {
                if (selfWallet.Address == address)
                {
                    return selfWallet;
                }
            }

            return null;
        }
    }
}