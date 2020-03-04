using System;
using Sining.Tools;

namespace Sining
{
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
}