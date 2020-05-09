using Sining.Event;

namespace Sining.Module
{
    [ComponentSystem]
    public class StartServerComponentAwakeSystem : AwakeSystem<StartServerComponent, Options>
    {
        protected override void Awake(StartServerComponent self, Options options)
        {
            AwakeAsync(options).Coroutine();
        }

        private async SVoid AwakeAsync(Options options)
        {
            await ServerHelper.Start(options);
        }
    }
}