using System;
using Sining.Module;

namespace Sining
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            App.Id = 1;

            App.Scene.AddComponent<NetworkComponent, NetworkProtocolType, string>(NetworkProtocolType.TCP,
                "127.0.0.1:8888");
        }
    }
}