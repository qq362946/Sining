using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Sining.DataStructure;
using Sining.Model;

namespace Sining.Module
{
    public class WithdrawManageComponent : Component
    {
        public static WithdrawManageComponent Instance;
        public Thread CurrentTask;
        public readonly OneToManyList<long, WithdrawInfo> WaitWithdrawInfos = new OneToManyList<long, WithdrawInfo>();
        public readonly Dictionary<long, WithdrawInfo> WithdrawInfos = new Dictionary<long, WithdrawInfo>();
        public readonly ConcurrentDictionary<long, WithdrawInfo> WithdrawInfoDic =
            new ConcurrentDictionary<long, WithdrawInfo>();
        public override void Dispose()
        {
            if (IsDispose)
            {
                return;
            }

            base.Dispose();
            
            CurrentTask.Abort();
            CurrentTask.Join();
            CurrentTask = null;

            foreach (var info in WaitWithdrawInfos.Values.SelectMany(withdrawInfo => withdrawInfo))
            {
                info.Dispose();
            }

            WaitWithdrawInfos.Clear();
            WithdrawInfos.Clear();
            
            foreach (var withdrawInfo in WithdrawInfoDic.Values)
            {
                withdrawInfo.Dispose();
            }
            
            WithdrawInfoDic.Clear();
        }
    }
}