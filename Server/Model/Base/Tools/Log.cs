using System;
using NLog;
using Sining.Tools;

namespace Sining
{
    public static class Log
    {
        private static readonly Logger Logger = LogManager.GetLogger("Logger");
        public static void Debug(string message)
        {
            Logger.Debug(message);
        }
        public static void Debug(object obj)
        {
            Debug(obj.Serialize());
        }
        public static void Info(string message)
        {
            Logger.Info(message);
        }
        public static void Info(object obj)
        {
            Info(obj.Serialize());
        }
        public static void Error(string message)
        {
            Logger.Error(message);
        }
        public static void Error(object obj)
        {
            Error(obj.Serialize());
        }
        public static void Error(Exception e)
        {
            Logger.Error(e.ToString());
        }
        public static void Trace(string message)
        {
            Logger.Trace(message);
        }
        public static void Trace(object obj)
        {
            Trace(obj.Serialize());
        }
        public static void Warning(string message)
        {
            Logger.Warn(message);
        }
        public static void Warning(object obj)
        {
            Warning(obj.Serialize());
        }
        public static void Fatal(string fatal)
        {
            Logger.Fatal(fatal);
        }
        public static void Fatal(object obj)
        {
            Fatal(obj.Serialize());
        }
        public static void Fatal(Exception e)
        {
            Logger.Fatal(e);
        }
    }
}