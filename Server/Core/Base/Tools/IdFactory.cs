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
             private static int _appId;
             
             private const int AppIdBits = 10;
             private const long AppIdMask = -1L ^ (-1L << AppIdBits);
             private static long _lastTimeStamp = -1L;
             private static long _sequence;
             private const int SequenceBits = 12; 
             private const long SequenceMask = -1L ^ (-1L << SequenceBits); 
             private const long Epoch = 453830400000L;
             private const int TimeStampLeftShift = SequenceBits + AppIdBits;
             private const int GetAppIdLeftShift = 64 - AppIdBits - SequenceBits;
             private const int GetAppIdRightShift = GetAppIdLeftShift + SequenceBits;
             private static readonly object LockObject = new object();
             
             /// <summary>
             /// 获取或设置APPID(一般是当前进程的ID)
             /// </summary>
             /// <exception cref="ArgumentException"></exception>
             public static int AppId
             {
                 get => _appId;
                 set
                 {
                     if (value > AppIdMask || value < 0)
                     {
                         throw new ArgumentException($"AppId can't be greater than {AppIdMask} or less than 0");
                     }
     
                     _appId = value;
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

                         if (_lastTimeStamp == timestamp)
                         {
                             _sequence = (_sequence + 1) & SequenceMask;

                             if (_sequence == 0)
                             {
                                 timestamp = WaitNextMillis();
                             }
                         }
                         else
                         {
                             _sequence = 0;
                         }

                         if (timestamp < _lastTimeStamp)
                         {
                             throw new Exception(
                                 $"Clock moved backwards.  Refusing to generate id for {_lastTimeStamp - timestamp} milliseconds");
                         }

                         _lastTimeStamp = timestamp;

                         return (timestamp - Epoch << TimeStampLeftShift) + (AppId << SequenceBits) + _sequence;
                     }
                 }
             }
     
             /// <summary>
             /// 根据ID获得APPID
             /// </summary>
             /// <param name="id"></param>
             /// <returns></returns>
             public static long GetAppId(long id)
             {
                 return id << GetAppIdLeftShift >> GetAppIdRightShift;
             }
     
             private static long WaitNextMillis()
             {
                 var timestamp = TimeHelper.Now;
                 while (timestamp <= _lastTimeStamp)
                 {
                     timestamp = TimeHelper.Now;
                 }
                 return timestamp;
             }
         }
}