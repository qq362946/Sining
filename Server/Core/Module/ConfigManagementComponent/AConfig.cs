using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Sining.Tools;

namespace Sining.Config
{
    public interface IAConfig : ISupportInitialize{ }

    public abstract class AConfig<T> : IAConfig where T : class
    {
        private const string ConfigDirectory = "../Config/";
        protected Dictionary<int, T> Configs;

        public T GetConfig(int id)
        {
            Configs.TryGetValue(id, out var config);

            return config;
        }
        public List<T> GetAllConfig()
        {
            return Configs.Values.ToList();
        }
        public virtual void BeginInit()
        {
            var configFile = Path.Combine(ConfigDirectory, $"{typeof(T).Name}.byte");

            if (!File.Exists(configFile))
            {
                throw new Exception($"{typeof(T).Name}.byte not found");
            }

            using var br = new BinaryReader(new FileStream(configFile, FileMode.Open, FileAccess.Read));

            var binaryFileClass = br.BaseStream.Deserialize<Dictionary<string, IConfig>>();

            Configs = new Dictionary<int, T>();

            foreach ((string id, object config) in binaryFileClass)
            {
                Configs.Add(int.Parse(id), (T) config);
            }
        }
        public virtual void EndInit() { }
    }
}