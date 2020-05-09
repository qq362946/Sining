namespace Sining.Module
{
    public class UserWallet : Component
    {
        public CoinConfigType CoinConfigType;
        public string Address;
        public decimal Money;
        public decimal Forzen;

        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }
            
            base.Dispose();
            CoinConfigType = CoinConfigType.None;
            Address = null;
            Money = 0;
            Forzen = 0;
        }
    }
}