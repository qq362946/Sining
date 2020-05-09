using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sining.Config;
using Sining.Event;
using Sining.Tools;

namespace Sining.Module
{
    public class ConfigManagementComponent : Component
    {
        public static ConfigManagementComponent Instance;
        private readonly ConcurrentDictionary<Type, IAConfig> _configs = new ConcurrentDictionary<Type, IAConfig>();
        public void Init()
        {
            var list = new List<Task>();

            foreach (var allTypes in AssemblyManagement.AllType.Values)
            {
                list.AddRange(allTypes.Where(d => d.IsDefined(typeof(ConfigAttribute), true))
                    .Select(type => Task.Run(() => LoadConfiguration(type))));
            }

            Task.WaitAll(list.ToArray());

            Instance = this;
        }

        public void ReLoad()
        {
            var list = new List<Task>();

            foreach (var allTypes in AssemblyManagement.AllType.Values)
            {
                list.AddRange(allTypes.Where(d => d.IsDefined(typeof(ConfigAttribute), true))
                    .Select(type => Task.Run(() =>
                    {
                        if (!_configs.TryGetValue(type, out var iAConfig))
                        {
                            LoadConfiguration(type);

                            return;
                        }

                        iAConfig.BeginClear();
                        iAConfig.BeginInit();
                    })));
            }

            Task.WaitAll(list.ToArray());
        }

        private void LoadConfiguration(Type type)
        {
            var obj = Activator.CreateInstance(type);

            if (!(obj is IAConfig iAConfig))
            {
                throw new Exception($"class: {type.Name} not inherit from IAConfig");
            }

            iAConfig.BeginInit();
            iAConfig.EndInit();
            _configs.TryAdd(type, iAConfig);
        }
    }
}