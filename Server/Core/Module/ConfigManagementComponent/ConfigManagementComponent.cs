using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sining.Config;
using Sining.Tools;

namespace Sining.Module
{
    public class ConfigManagementComponent : Component
    {
        public void Init()
        {
            Task.WaitAll(
                AssemblyManagement.AllType.Where(d => d.IsDefined(typeof(ConfigAttribute), true))
                    .Select(type => Task.Run(() => LoadConfiguration(type))).ToArray());
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
        }
    }
}