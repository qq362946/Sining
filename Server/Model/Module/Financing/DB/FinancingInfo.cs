namespace Sining.Model
{
    public class FinancingInfo : Component
    {
        public long UserId;                                     // 用户Id
        public int CoinId;                                      // 币类型
        public string CoinName;                                 // 币名称
        public long AddTime;                                    // 创建时间
        public FinancingConfigType FinancingConfigType;         // 资金变动类型ID
        public string MoldTitle;                                // 资金变动类型名称
        public decimal Money;                                   // 充值金额
        public decimal Balance;                                 // 余额
        public string Remark;                                   // 备注

        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }
            
            base.Dispose();
            UserId = 0;
            CoinId = 0;
            CoinName = null;
            AddTime = 0;
            FinancingConfigType = FinancingConfigType.None;
            MoldTitle = null;
            Money = 0;
            Balance = 0;
            Remark = null;
        }
    }
}