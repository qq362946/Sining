using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sining.Tools
{
    public static class AssemblyManagement
    {
        public static readonly List<Type> AllType = new List<Type>();

        public static void Init()
        {
            Init("Server.Hotfix.dll");
        }

        private static void Init(params string[] assemblyName)
        {
            // 清除当前所有信息
            
            AllType.Clear();
            
            // 加载Core程序集

            AllType.AddRange(Assembly.GetExecutingAssembly().GetTypes());

            foreach (var assembly in assemblyName)
            {
                AllType.AddRange(Assembly.LoadFrom(assembly).GetTypes());
            }
        }
    }
}