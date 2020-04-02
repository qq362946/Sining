using System;
using Sining.Config;
using Sining.Module;

namespace Sining
{
    public static class SceneFactory
    {
        public static void Create(Scene scene)
        {
            switch (scene.SceneType)
            {
                case SceneType.RealmAccount:
                    // // 释放锁仓组件
                    // scene.AddComponent<ReleaseWarehouseComponent>();
                    break;
                default:
                    throw new Exception($"No SceneType found for {(int) scene.SceneType}");
            }
        }
    }
}