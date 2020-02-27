using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Server.Network;
using Sining.DataStructure;
using Sining.Network;

namespace Sining.Event
{
    public static class EventManagement
    {
        #region Member variables

        public static readonly List<Type> AllType = new List<Type>();

        private static readonly OneToManyList<Type, IAwakeSystem> AwakeSystem = new OneToManyList<Type, IAwakeSystem>(0);
        
        private static readonly OneToManyList<Type, IDestroySystem> DestroySystem = new OneToManyList<Type, IDestroySystem>(0);
        
        private static readonly OneToManyList<string, object> EventSystem = new OneToManyList<string, object>(0);

        #endregion

        #region Initialization

        public static void Initialization()
        {
            Initialization("Server.Model.dll", "Server.Hotfix.dll");
        }

        public static void Initialization(params string[] assemblyName)
        {
            // 清除当前所有信息
            
            Clear();
            
            // 加载Core程序集

            AddAssembly(typeof(EventManagement).Assembly.GetTypes());

            foreach (var assembly in assemblyName)
            {
                AddAssembly(Assembly.LoadFrom(assembly).GetTypes());
            }
        }
        
        private static void AddAssembly(Type[] types)
        {
            AllType.AddRange(types);

            foreach (var type in types.Where(d => d.IsDefined(typeof(BaseAttribute), true)))
            {
                var obj = Activator.CreateInstance(type);

                // 组件事件

                if (type.IsDefined(typeof(ComponentSystemAttribute), true))
                {
                    switch (obj)
                    {
                        case IAwakeSystem awakeSystem:
                            AwakeSystem.Add(awakeSystem.Type(), awakeSystem);
                            break;
                        case IDestroySystem disposeSystem:
                            DestroySystem.Add(disposeSystem.Type(), disposeSystem);
                            break;
                    }

                    continue;
                }

                // 一般处理事件

                if (type.IsDefined(typeof(EventSystemAttribute), true))
                {
                    foreach (var customAttribute in type.GetCustomAttributes<EventSystemAttribute>(true))
                    {
                        EventSystem.Add(customAttribute.EventType, obj);
                    }

                    continue;
                }

                // 消息事件

                if (type.IsDefined(typeof(MessageSystemAttribute), true))
                {
                    if (!(obj is IMessageHandler messageHandler))
                    {
                        throw new Exception($"message handle {type.Name} 需要继承 IMessageHandler");
                    }

                    MessageDispatcher.AddHandler(messageHandler.Type(), messageHandler);
                }

                // 网络协议

                if (type.IsDefined(typeof(MessageAttribute), true))
                {
                    NetworkProtocolManage.Add(type.GetCustomAttribute<MessageAttribute>().Opcode, type);
                }
            }
        }

        #endregion

        #region ComponentSystemEvent

        public static void Awake<T>(T t) where T : Component
        {
            if (!AwakeSystem.TryGetValue(typeof(T), out var list)) return;

            RunEvent(list, t);
        }

        public static void Awake<T, T1>(T t, T1 a) where T : Component
        {
            if (!AwakeSystem.TryGetValue(typeof(T), out var list)) return;

            RunEvent(list, t, a);
        }

        public static void Awake<T, T1, T2>(T t, T1 a, T2 b) where T : Component
        {
            if (!AwakeSystem.TryGetValue(typeof(T), out var list)) return;

            RunEvent(list, t, a, b);
        }

        public static void Awake<T, T1, T2, T3>(T t, T1 a, T2 b, T3 c) where T : Component
        {
            if (!AwakeSystem.TryGetValue(typeof(T), out var list)) return;

            RunEvent(list, t, a, b, c);
        }

        public static void Awake<T, T1, T2, T3, T4>(T t, T1 a, T2 b, T3 c, T4 d) where T : Component
        {
            if (!AwakeSystem.TryGetValue(typeof(T), out var list)) return;

            RunEvent(list, t, a, b, c, d);
        }

        public static void Destroy(Component component)
        {
            var type = component.GetType();

            if (!DestroySystem.TryGetValue(type, out var list) || list == null) return;

            foreach (var destroySystem in list)
            {
                destroySystem.Run(component);
            }
        }

        #endregion

        #region EventSystem

        public static void Publish(string eventName)
        {
            if (!EventSystem.TryGetValue(eventName, out var list)) return;

            RunEvent(list);
        }

        public static void Publish<T>(string eventName, T a)
        {
            if (!EventSystem.TryGetValue(eventName, out var list)) return;

            RunEvent(list, a);
        }

        public static void Publish<T, T1>(string eventName, T a, T1 b)
        {
            if (!EventSystem.TryGetValue(eventName, out var list)) return;

            RunEvent(list, a, b);
        }

        public static void Publish<T, T1, T2>(string eventName, T a, T1 b, T2 c)
        {
            if (!EventSystem.TryGetValue(eventName, out var list)) return;

            RunEvent(list, a, b, c);
        }

        public static void Publish<T, T1, T2, T3>(string eventName, T a, T1 b, T2 c, T3 d)
        {
            if (!EventSystem.TryGetValue(eventName, out var list)) return;

            RunEvent(list, a, b, c, d);
        }

        public static void Publish<T, T1, T2, T3, T4>(string eventName, T a, T1 b, T2 c, T3 d, T4 e)
        {
            if (!EventSystem.TryGetValue(eventName, out var list)) return;

            RunEvent(list, a, b, c, d, e);
        }

        #endregion

        #region RunEvent

        private static void RunEvent<TEventType>(IEnumerable<TEventType> list)
        {
            foreach (var eventSystem in list.OfType<IEvent>())
            {
                try
                {
                    eventSystem.Run();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void RunEvent<TEventType, T>(IEnumerable<TEventType> list, T t)
        {
            foreach (var eventSystem in list.OfType<IEvent<T>>())
            {
                try
                {
                    eventSystem.Run(t);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void RunEvent<TEventType, T, T1>(IEnumerable<TEventType> list, T t, T1 a)
        {
            foreach (var eventSystem in list.OfType<IEvent<T, T1>>())
            {
                try
                {
                    eventSystem.Run(t, a);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void RunEvent<TEventType, T, T1, T2>(IEnumerable<TEventType> list, T t, T1 a, T2 b)
        {
            foreach (var eventSystem in list.OfType<IEvent<T, T1, T2>>())
            {
                try
                {
                    eventSystem.Run(t, a, b);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void RunEvent<TEventType, T, T1, T2, T3>(IEnumerable<TEventType> list, T t, T1 a, T2 b, T3 c)
        {
            foreach (var eventSystem in list.OfType<IEvent<T, T1, T2, T3>>())
            {
                try
                {
                    eventSystem.Run(t, a, b, c);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void RunEvent<TEventType, T, T1, T2, T3, T4>(IEnumerable<TEventType> list, T t, T1 a, T2 b, T3 c,
            T4 d)
        {
            foreach (var eventSystem in list.OfType<IEvent<T, T1, T2, T3, T4>>())
            {
                try
                {
                    eventSystem.Run(t, a, b, c, d);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        
        #endregion

        private static void Clear()
        {
            AllType.Clear();
            AwakeSystem.Clear();
            DestroySystem.Clear();
            EventSystem.Clear();
            NetworkProtocolManage.Clear();
        }
    }
}