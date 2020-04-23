using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sining.DataStructure;

namespace Sining.Tools
{
    public static class AssemblyManagement
    {
        public static readonly OneToManyList<string, Type> AllType = new OneToManyList<string, Type>();
        public const string Model = "Model";
        public const string Hotfix = "Hotfix";
#if SiningClient
        public static void Init()
        {
            Init("Client.Hotfix.dll");
        }
#else
        public static void Init()
        {
            Init("Server.Hotfix.dll");
        }
#endif

        private static void Init(params string[] assemblyName)
        {
            // 清除当前所有信息

            AllType.Clear();

            // 加载Model程序集

            AllType.Add(Model, Assembly.GetExecutingAssembly().GetTypes().ToList());

            foreach (var assembly in assemblyName)
            {
                Load(assembly);
            }
        }

        public static void ReLoadHotfix()
        {
            AllType.RemoveKey(Hotfix);

            Load("Server.Hotfix.dll");
        }

        private static void Load(string dllPath)
        {
            var fileData = File.ReadAllBytes(dllPath);

            AllType.Add(Hotfix, Assembly.Load(fileData).GetTypes().ToList());
        }
    }
}