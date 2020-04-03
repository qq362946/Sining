using System;
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
                    break;
                case SceneType.Location:
                    break;
                case SceneType.Map:
                    break;
                case SceneType.MessageForwarding:
                    break;
                default:
                    throw new Exception($"No SceneType found for {(int) scene.SceneType}");
            }
        }
    }
}