using CommandLine;
using Sining.Event;
using Sining.Tools;

namespace Sining
{
    public class Options : Component
    {
        [Option("Server", Required = false, Default = 0)]
        public int Server { get; set; }

        public void Initialization()
        {
            if (Server == 0)
            {
                Log.Info("开始启动服务器，请稍等...");
                
                Log.Info("加载服务器配置文件...");

                
                
                return;
            }
        }
    }
}