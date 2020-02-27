using System;
using NLog;

namespace Sining
{
    public static class Log
    {
        private static readonly Logger Logger = LogManager.GetLogger("Logger");

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            Logger.Error(message);
        }
        
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="e"></param>
        public static void Error(Exception e)
        {
            Logger.Error(e.ToString());
        }
        
        /// <summary>
        /// 跟踪
        /// </summary>
        /// <param name="message"></param>
        public static void Trace(string message)
        {
            Logger.Trace(message);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="message"></param>
        public static void Warning(string message)
        {
            Logger.Warn(message);
        }
        
        /// <summary>
        /// 严重错误
        /// </summary>
        /// <param name="fatal"></param>
        public static void Fatal(string fatal)
        {
            Logger.Fatal(fatal);
        }
        
        /// <summary>
        /// 严重错误
        /// </summary>
        /// <param name="e"></param>
        public static void Fatal(Exception e)
        {
            Logger.Fatal(e);
        }
    }
}