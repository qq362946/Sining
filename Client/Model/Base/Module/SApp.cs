using Sining.Tools;

namespace Sining.Module
{
    public static class SApp
    { 
        static SApp()
        {
            IdFactory.AppId = 1;
        }
        
        public static void Init(){}

        private static Scene _scene;

        public static Scene Scene
        {
            get
            {
                if (_scene != null) return _scene;

                return _scene = new Scene();
            }
        }
    }
}