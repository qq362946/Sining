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
                    break;
                case SceneType.MessageForwarding:
                    break;
                case SceneType.Map:
                    break;
                case SceneType.Location:
                    break;
                default:
                    throw new Exception($"No SceneType found for {(int) scene.SceneType}");
            }
        }
    }
}