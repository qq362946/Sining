using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Export2Other
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var tasks = new List<Task>()
                {
                    Task.Run(ServerConfigToEnum.Run),
                    Task.Run(SceneConfigToEnum.Run),
                    Task.Run(ConstValueHelper.Run)
                };

                Task.WaitAll(tasks.ToArray());

                Console.WriteLine("export succeeded!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}