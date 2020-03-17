using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Network;
using Sining.Module;
using Sining.Network;

namespace Sining.Tools
{
    public static class SiningSystem
    {
        public static void Init()
        {
            // 反射需要的类库
            AssemblyManagement.Init();
            // 初始化Bson库
            SerializationHelper.Init();
            // 初始化组件事件
            var componentManagement = ComponentFactory.CreateOnly<ComponentManagement>(eventSystem: false);
            componentManagement.Init();
            SApp.Scene.AddComponent(componentManagement);

            var tasks = new List<Task>
            {
                // 初始化普通事件
                Task.Run(() => SApp.Scene.AddComponent<SystemEventComponent>().Init()),
                // 初始化网络协议
                Task.Run(() => SApp.Scene.AddComponent<NetworkProtocolManagement>().Init()),
                // 初始化网络协议处理程序
                Task.Run(() => SApp.Scene.AddComponent<MessageDispatcherManagement>().Init()),
                // 初始化配置文件
                Task.Run(() => SApp.Scene.AddComponent<ConfigManagementComponent>().Init())
            };
            // 只是为了执行下静态的构造函数
            SApp.Init();
            // 等待线程全部处理完毕
            Task.WaitAll(tasks.ToArray());
            // 逻辑处理组件
            SApp.Scene.AddComponent<TaskProcessingComponent>();
        }
    }
}