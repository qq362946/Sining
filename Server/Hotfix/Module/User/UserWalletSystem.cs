using Sining.Event;

namespace Sining.Module
{
    [ComponentSystem]
    public class UserWalletAwakeSystem : AwakeSystem<UserWallet, CoinConfigType, string, decimal>
    {
        protected override void Awake(UserWallet self, CoinConfigType coinConfigType, string address, decimal money)
        {
            self.CoinConfigType = coinConfigType;
            self.Address = address;
            self.Money = money;
        }
    }
}