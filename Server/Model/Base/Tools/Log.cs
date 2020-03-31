using System;
using MongoDB.Bson;
#if !SiningClient
using NLog;
#endif
using Sining.Tools;

namespace Sining
{
#if SiningClient
    public static class Log
    {
        public static void Debug(string message)
        {
            Console.WriteLine(message);
        }
        public static void Debug(object obj)
        {
            Debug(obj.ToJson());
        }
        public static void Info(string message)
        {
            Console.WriteLine(message);
        }
        public static void Info(object obj)
        {
            Info(obj.ToJson());
        }
        public static void Error(string message)
        {
            Console.WriteLine(message);
        }
        public static void Error(object obj)
        {
            Error(obj.ToJson());
        }
        public static void Error(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        public static void Trace(string message)
        {
            Console.WriteLine(message);
        }
        public static void Trace(object obj)
        {
            Trace(obj.ToJson());
        }
        public static void Warning(string message)
        {
            Console.WriteLine(message);
        }
        public static void Warning(object obj)
        {
            Warning(obj.ToJson());
        }
        public static void Fatal(string fatal)
        {
            Console.WriteLine(fatal);
        }
        public static void Fatal(object obj)
        {
            Fatal(obj.ToJson());
        }
        public static void Fatal(Exception e)
        {
            Console.WriteLine(e);
        }
    }
#else
    public static class Log
    {
        private static readonly Logger Logger = LogManager.GetLogger("Logger");
        public static void Debug(string message)
        {
            Logger.Debug(message);
        }
        public static void Debug(object obj)
        {
            Debug(obj.ToJson());
        }
        public static void Info(string message)
        {
            Logger.Info(message);
        }
        public static void Info(object obj)
        {
            Info(obj.ToJson());
        }
        public static void Error(string message)
        {
            Logger.Error(message);
        }
        public static void Error(object obj)
        {
            Error(obj.ToJson());
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
            Trace(obj.ToJson());
        }
        public static void Warning(string message)
        {
            Logger.Warn(message);
        }
        public static void Warning(object obj)
        {
            Warning(obj.ToJson());
        }
        public static void Fatal(string fatal)
        {
            Logger.Fatal(fatal);
        }
        public static void Fatal(object obj)
        {
            Fatal(obj.ToJson());
        }
        public static void Fatal(Exception e)
        {
            Logger.Fatal(e);
        }
    }
#endif
}