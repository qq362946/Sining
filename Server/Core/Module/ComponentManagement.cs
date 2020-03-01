using System;
using System.Linq;
using Sining.DataStructure;
using Sining.Event;
using Sining.Tools;

namespace Sining.Module
{
    public class ComponentManagement : Component
    {
        public static ComponentManagement Instance;

        private readonly OneToManyList<Type, IAwakeSystem> _awakeSystem = new OneToManyList<Type, IAwakeSystem>(0);
        
        private readonly OneToManyList<Type, IDestroySystem> _destroySystem = new OneToManyList<Type, IDestroySystem>(0);

        public void Init()
        {
            foreach (var type in AssemblyManagement.AllType.Where(d =>
                d.IsDefined(typeof(ComponentSystemAttribute), true)))
            {
                var obj = Activator.CreateInstance(type);

                switch (obj)
                {
                    case IAwakeSystem awakeSystem:
                        _awakeSystem.Add(awakeSystem.Type(), awakeSystem);
                        break;
                    case IDestroySystem disposeSystem:
                        _destroySystem.Add(disposeSystem.Type(), disposeSystem);
                        break;
                }
            }

            Instance = this;
        }

        public void Awake<T>(T t) where T : Component
        {
            if (!_awakeSystem.TryGetValue(typeof(T), out var list)) return;
            
            foreach (var eventSystem in list.OfType<IEvent>())
            {
                try
                {
                    eventSystem.Run();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Awake<T, T1>(T t, T1 a) where T : Component
        {
            if (!_awakeSystem.TryGetValue(typeof(T), out var list)) return;

            foreach (var eventSystem in list.OfType<IEvent<T>>())
            {
                try
                {
                    eventSystem.Run(t);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Awake<T, T1, T2>(T t, T1 a, T2 b) where T : Component
        {
            if (!_awakeSystem.TryGetValue(typeof(T), out var list)) return;

            foreach (var eventSystem in list.OfType<IEvent<T, T1>>())
            {
                try
                {
                    eventSystem.Run(t, a);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Awake<T, T1, T2, T3>(T t, T1 a, T2 b, T3 c) where T : Component
        {
            if (!_awakeSystem.TryGetValue(typeof(T), out var list)) return;

            foreach (var eventSystem in list.OfType<IEvent<T, T1, T2>>())
            {
                try
                {
                    eventSystem.Run(t, a, b);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Awake<T, T1, T2, T3, T4>(T t, T1 a, T2 b, T3 c, T4 d) where T : Component
        {
            if (!_awakeSystem.TryGetValue(typeof(T), out var list)) return;

            foreach (var eventSystem in list.OfType<IEvent<T, T1, T2, T3>>())
            {
                try
                {
                    eventSystem.Run(t, a, b, c);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Destroy(Component component)
        {
            var type = component.GetType();

            if (!_destroySystem.TryGetValue(type, out var list) || list == null) return;

            foreach (var destroySystem in list)
            {
                destroySystem.Run(component);
            }
        }

        public void Clear()
        {
            _awakeSystem.Clear();
            _destroySystem.Clear();
        }

        public override void Dispose()
        {
            if (IsDispose) return;

            Clear();
            
            base.Dispose();
        }
    }
}