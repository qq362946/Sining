using System;
using System.Threading;
using System.Threading.Tasks;
using Sining.Event;
using Sining.Tools;

namespace Sining.Module
{
    [ComponentSystem]
    public class ConsoleComponentAwakeSystem : AwakeSystem<ConsoleComponent>
    {
        protected override void Awake(ConsoleComponent self)
        {
            self.Start().Coroutine();
        }
    }
    public class ConsoleComponent : Component
    {
        private CancellationTokenSource _cancellationTokenSource;
        public async SVoid Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                for (;;)
                {
                    try
                    {
                        var line = await Task.Factory.StartNew(() => Console.In.ReadLine(), _cancellationTokenSource.Token);
                        
                        switch (line.Trim())
                        {
                            case "repl reload":
                                SiningSystem.ReLoad();
                                Log.Debug("类库重新加载成功");
                                break;
                            case "":
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}