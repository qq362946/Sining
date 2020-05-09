namespace Sining.Model
{
    public class WithdrawInfo : Component
    {
        public long UserId;                       // 用户ID
        public string Address;                    // 提现地址
        public decimal Money;                     // 提现金额
        public CoinConfigType FeeCoin;            // 手续费币种
        public string FeeCoinEname;               // 手续费币种名称
        public decimal Fee;                       // 手续费
        public decimal Real;                      // 实际到账
        public decimal Balance;                   // 提现后余额
        public int ProcessMold;                   // 处理模式，0区块处理，1外部处理
        public string Hash;                       // 交易Hash值
        public int Status;                        // 状态，-1,驳回,0待处理,1处理中,2已处理
        public long AddTime;                      // 申请时间
        public long ProcessTime;                  // 处理时间
        public string WithdrawInfoRemark;         // 自动处理未成功时返回的错误信息
        public string CoinName;                   // 提现币种名称
        public CoinConfigType CoinId;             // 提现币种
        public string Remark;                     // 提现备注

        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }

            base.Dispose();

            CoinId = CoinConfigType.None;
            UserId = 0;
            Address = null;
            Money = 0;
            FeeCoin = CoinConfigType.None;
            FeeCoinEname = null;
            Fee = 0;
            Real = 0;
            Balance = 0;
            ProcessMold = 0;
            Hash = null;
            Status = 0;
            AddTime = 0;
            ProcessTime = 0;
            WithdrawInfoRemark = null;
            CoinName = null;
            Remark = null;
        }
    }
}