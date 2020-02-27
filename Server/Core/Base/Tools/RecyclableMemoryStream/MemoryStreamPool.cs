using System.Threading;
using Microsoft.IO;

namespace Sining.Tools
{
    public class MemoryStreamPool
    {
        private static RecyclableMemoryStreamManager _instance;

        public static RecyclableMemoryStreamManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Interlocked.CompareExchange(ref _instance, new RecyclableMemoryStreamManager(), null);
                }

                return _instance;
            }
        }
        
        private MemoryStreamPool() {}
    }
}