using NLog;
using Sining.Tools;

namespace Sining
{
    public static class App
    {
        private static int _id;

        public static int Id
        {
            get => _id;
            set
            {
                IdFactory.AppId = value;
                LogManager.Configuration.Variables["appId"] = $"{IdFactory.AppId:0000}";
                _id = value;
            }
        }

        private static Scene _scene;

        public static Scene Scene
        {
            get
            {
                if (_scene != null) return _scene;

                return _scene = ComponentFactory.Create<Scene>();
            }
        }
    }
}