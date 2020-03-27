using System.Collections.Generic;
using Sining.Config;

namespace Sining.Module
{
    public static class DBHelper
    {
        private static readonly Dictionary<int, DBComponent> DbComponents = new Dictionary<int, DBComponent>();
        public static void AddDbComponent(this Scene scene)
        {
            if (DbComponents.ContainsKey(scene.SceneConfig.Zone))
            {
                return;
            }

            var zone = ZoneConfigData.Instance.GetConfig(scene.SceneConfig.Zone);

            var dbComponent =
                ComponentFactory.Create<DBComponent, string, string>(SApp.Scene, zone.DbConnection, zone.DbName);

            DbComponents.Add(scene.SceneConfig.Zone, dbComponent);
        }
        public static DBComponent DataBase(this Scene scene)
        {
            DbComponents.TryGetValue(scene.SceneConfig.Zone, out var dbComponent);

            return dbComponent;
        }
        public static DBComponent DataBase<T>(this T component) where T : Component
        {
            return component.Scene.DataBase();
        }
        public static DBComponent DataBase(int sceneId)
        {
            var scene = SceneManagementComponent.Instance.GetScene(sceneId);

            return scene.DataBase();
        }
    }
}