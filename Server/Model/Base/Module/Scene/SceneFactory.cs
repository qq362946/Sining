using System;

namespace Sining
{
    public static class SceneFactory
    {
        public static void Create(Scene scene)
        {
            switch (scene.SceneType)
            {
                case SceneType.RealmAccount:
                    break;
                default:
                    throw new Exception($"No SceneType found for {(int) scene.SceneType}");
            }
        }
    }
}