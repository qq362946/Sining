using System;
using System.Threading;

namespace Sining.Tools
{
    /// <summary>
    /// 生成不重复ID
    /// 1、AppId最大为1024个，基本很少有1024个进程，就算有可以用战区来解决。
    /// 2、解决了每毫秒产生ID数量太多会重复的问题。
    /// 3、经过测试每秒能够产生将近26万个ID
    /// </summary>
    public static class IdFactory
    {
        private static long __appId;

        private const int AppIdBits = 10;
        private const long AppIdMask = -1L ^ (-1L << AppIdBits);
        private static long __lastTimeStamp = -1L;
        private static long __sequence;
        private const int SequenceBits = 12;
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);
        private const long Epoch = 453830400000L;
        private const int AppIdLeftShift = 64 - SequenceBits;
        private const int SequenceLeftShift = AppIdLeftShift - SequenceBits;
        private static readonly object LockObject = new object();

        /// <summary>
        /// 获取或设置APPID(一般是当前进程的ID)
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static long AppId
        {
            get => __appId;
            set
            {
                if (value > AppIdMask || value < 0)
                {
                    throw new ArgumentException($"AppId can't be greater than {AppIdMask} or less than 0");
                }

                __appId = value;
            }
        }

        /// <summary>
        /// 获取一个新的ID
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static long NextId
        {
            get
            {
                lock (LockObject)
                {
                    var timestamp = TimeHelper.Now;

                    if (__lastTimeStamp == timestamp)
                    {
                        __sequence = (__sequence + 1) & SequenceMask;

                        if (__sequence == 0)
                        {
                            timestamp = WaitNextMillis();
                        }
                    }
                    else
                    {
                        __sequence = 0;
                    }

                    if (timestamp < __lastTimeStamp)
                    {
                        throw new Exception(
                            $"Clock moved backwards. Refusing to generate id for {__lastTimeStamp - timestamp} milliseconds");
                    }

                    __lastTimeStamp = timestamp;

                    return (AppId << AppIdLeftShift) + (__sequence << SequenceLeftShift) + (timestamp - Epoch);
                }
            }
        }

        public static long GetAppId(this long id)
        {
            return id >> AppIdLeftShift;
        }

        private static long WaitNextMillis()
        {
            var timestamp = TimeHelper.Now;
            while (timestamp <= __lastTimeStamp)
            {
                timestamp = TimeHelper.Now;
            }

            return timestamp;
        }
    }
}