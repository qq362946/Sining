using System;
using System.Linq;
using Sining.DataStructure;
using Sining.Event;
using Sining.Tools;
using System.Reflection;

namespace Sining.Module
{
    public class SystemEventComponent : Component
    {
        public static SystemEventComponent Instance;
        
        private readonly OneToManyList<string, object> _eventSystem = new OneToManyList<string, object>(0);

        public void Init()
        {
            foreach (var type in AssemblyManagement.AllType.Where(d =>
                d.IsDefined(typeof(EventSystemAttribute), true)))
            {
                var obj = Activator.CreateInstance(type);

                foreach (var customAttribute in type.GetCustomAttributes<EventSystemAttribute>(true))
                {
                    _eventSystem.Add(customAttribute.EventType, obj);
                }
            }

            Instance = this;
        }
        
        public void Publish(string eventName)
        {
            if (!_eventSystem.TryGetValue(eventName, out var list)) return;

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

        public void Publish<T>(string eventName, T a)
        {
            if (!_eventSystem.TryGetValue(eventName, out var list)) return;

            foreach (var eventSystem in list.OfType<IEvent<T>>())
            {
                try
                {
                    eventSystem.Run(a);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Publish<T, T1>(string eventName, T a, T1 b)
        {
            if (!_eventSystem.TryGetValue(eventName, out var list)) return;

            foreach (var eventSystem in list.OfType<IEvent<T, T1>>())
            {
                try
                {
                    eventSystem.Run(a, b);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Publish<T, T1, T2>(string eventName, T a, T1 b, T2 c)
        {
            if (!_eventSystem.TryGetValue(eventName, out var list)) return;

            foreach (var eventSystem in list.OfType<IEvent<T, T1, T2>>())
            {
                try
                {
                    eventSystem.Run(a, b, c);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Publish<T, T1, T2, T3>(string eventName, T a, T1 b, T2 c, T3 d)
        {
            if (!_eventSystem.TryGetValue(eventName, out var list)) return;

            foreach (var eventSystem in list.OfType<IEvent<T, T1, T2, T3>>())
            {
                try
                {
                    eventSystem.Run(a, b, c, d);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Publish<T, T1, T2, T3, T4>(string eventName, T a, T1 b, T2 c, T3 d, T4 e)
        {
            if (!_eventSystem.TryGetValue(eventName, out var list)) return;

            foreach (var eventSystem in list.OfType<IEvent<T, T1, T2, T3, T4>>())
            {
                try
                {
                    eventSystem.Run(a, b, c, d, e);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        public void Clear()
        {
            _eventSystem.Clear();
        }

        public override void Dispose()
        {
            if(IsDispose) return;

            Clear();
            
            base.Dispose();
        }
    }
}