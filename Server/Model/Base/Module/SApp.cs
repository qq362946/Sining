using NLog;
using Sining.Config;
using Sining.Tools;

namespace Sining
{
    public static class SApp
    {
        private static int __id;
        public static int Id
        {
            get => __id;
            set
            {
                IdFactory.AppId = value;
                LogManager.Configuration.Variables["appId"] = $"{IdFactory.AppId:0000}";
                ServerType = (ServerType) value;
                ServerConfig = ServerConfigData.Instance.GetConfig(value);
                __id = value;
            }
        }
        public static ServerConfig ServerConfig { get; private set; }
        public static ServerType ServerType { get; private set; }
        private static Scene __scene;
        public static Scene Scene
        {
            get
            {
                if (__scene != null) return __scene;

                return __scene = new Scene();
            }
        }
    }
}