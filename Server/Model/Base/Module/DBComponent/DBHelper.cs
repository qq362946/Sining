using System.Collections.Generic;
using Sining.Config;

namespace Sining.Module
{
    public static class DBHelper
    {
        private static readonly Dictionary<int, ADBComponent> DbComponents = new Dictionary<int, ADBComponent>();
        public static void AddDbComponent(this Scene scene)
        {
            if (DbComponents.ContainsKey(scene.SceneConfig.Zone))
            {
                return;
            }

            var zone = ZoneConfigData.Instance.GetConfig(scene.SceneConfig.Zone);

            ADBComponent dbComponent;
            
            if (zone.DbType == "MongoDB")
            {
                dbComponent =
                    ComponentFactory.Create<MongoDBComponent, string, string>(SApp.Scene, zone.DbConnection,
                        zone.DbName);
            }
            else
            {
                dbComponent =
                    ComponentFactory.Create<SqlDBComponent, string, string, string>(SApp.Scene, zone.DbConnection,
                        zone.DbType, zone.DbName);
            }

            DbComponents.Add(scene.SceneConfig.Zone, dbComponent);
        }
        public static void Init()
        {
            foreach (var dbComponentsValue in DbComponents.Values)
            {
                dbComponentsValue.Init();
            }
            
            Log.Debug("所有数据库初始化成功！");
        }
        public static ADBComponent DataBase(this Scene scene)
        {
            DbComponents.TryGetValue(scene.SceneConfig.Zone, out var dbComponent);

            return dbComponent;
        }
        public static ADBComponent DataBase<T>(this T component) where T : Component
        {
            return component.Scene.DataBase();
        }
        public static ADBComponent DataBase(int sceneId)
        {
            var scene = SceneManagementComponent.Instance.GetScene(sceneId);

            return scene.DataBase();
        }
    }
}