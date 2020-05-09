namespace Sining.Module
{
    public class CoinRechargeInfo : Component
    {
        public string Hash;                    // 交易的hash值
        public long Block;                     // 区块位置
        public string From;
        public string Address;                 // 充值地址
        public decimal Money;                  // 充值金额
        public long AddTime;                   // 创建时间
        public int Confirms;                   // 确认数
        public int CoinId;                     // 币种ID
        public long UserId;                    // 用户ID
        public string CoinName;                // 币名称
        public bool RechargeStatus = false;    // 充值状态

        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }
            
            base.Dispose();

            Hash = null;
            Block = 0;
            From = null;
            Address = null;
            Money = 0;
            AddTime = 0;
            Confirms = 0;
            CoinId = 0;
            CoinName = null;
            RechargeStatus = false;
            UserId = 0;
        }
    }
}