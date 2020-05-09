using Sining.Module;

namespace Sining.Tools
{
    public static class SingleDataHeler
    {
        public static async STask<T> Get<T>(Scene scene, int singleDataType, string collectionName = "SingleData") where T : Component
        {
            return await scene.DataBase().Query<T>(singleDataType, collectionName);
        }

        public static async STask Save(Component component, string collectionName = "SingleData")
        {
            await component.DataBase().Save(component, collectionName);
        }

        public static async STask Save(object transactionSession, Component component,
            string collectionName = "SingleData")
        {
            await component.DataBase().Save(transactionSession, component, collectionName);
        }

        public static void SaveNoWait(Component component, string collectionName = "SingleData")
        {
            component.DataBase().SaveNotWait(component, collectionName).Coroutine();
        }
    }
}