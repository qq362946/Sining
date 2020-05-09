using System.Collections.Generic;
using System.Threading;

namespace Sining.Module
{
    public class CbeCoinComponent : MoacCoinComponent
    {
        public CbeConfigComponent CbeConfigComponent;
        public readonly HashSet<CoinRechargeInfo> CoinRechargeInfos =
            new HashSet<CoinRechargeInfo>();
        public readonly Dictionary<string, CoinRechargeInfo> WaitCoinRechargeInfos =
            new Dictionary<string, CoinRechargeInfo>();
        public Thread CurrentTask;

        public void Clear()
        {
            foreach (var coinRechargeInfo in CoinRechargeInfos)
            {
                coinRechargeInfo.Dispose();
            }
            
            CoinRechargeInfos.Clear();
            
            foreach (var waitCoinRechargeInfo in WaitCoinRechargeInfos.Values)
            {
                waitCoinRechargeInfo.Dispose();
            }
            
            WaitCoinRechargeInfos.Clear();
        }
        
        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }

            base.Dispose();
            
            CbeConfigComponent.Dispose();
            CbeConfigComponent = null;
            CurrentTask.Abort();
            CurrentTask.Join();
            CurrentTask = null;
            Clear();
        }
    }
}