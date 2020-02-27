using System;
using Sining.DataStructure;
using Sining.Event;
using Sining.Tools;

namespace Sining
{
    public class ComponentFactory
    {
        private static readonly OneToManyQueue<Type, Component> Pool = new OneToManyQueue<Type, Component>(0);

        private static readonly object LockObject = new object();
        
        #region CreateOnly

        public static T CreateOnly<T>(Component parent = null, bool isChild = false, bool eventSystem = true)
            where T : Component, new()
        {
            T component = null;

            try
            {
                lock (LockObject)
                {
                    component = (T) Pool.Dequeue(typeof(T)) ?? new T();

                    component.Initialization(parent, isChild, false);
                }

                if (eventSystem) EventManagement.Awake(component);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return component;
        }

        public static T CreateOnly<T, T1>(T1 a, Component parent = null, bool isChild = false)
            where T : Component, new()
        {
            var component = CreateOnly<T>(parent, isChild, false);

            EventManagement.Awake(component, a);

            return component;
        }

        public static T CreateOnly<T, T1, T2>(T1 a, T2 b, Component parent = null, bool isChild = false)
            where T : Component, new()
        {
            var component = CreateOnly<T>(parent, isChild, false);

            EventManagement.Awake(component, a, b);

            return component;
        }

        public static T CreateOnly<T, T1, T2, T3>(T1 a, T2 b, T3 c, Component parent = null, bool isChild = false)
            where T : Component, new()
        {
            var component = CreateOnly<T>(parent, isChild, false);

            EventManagement.Awake(component, a, b, c);

            return component;
        }

        public static T CreateOnly<T, T1, T2, T3, T4>(T1 a, T2 b, T3 c, T4 d, Component parent = null,
            bool isChild = false)
            where T : Component, new()
        {
            var component = CreateOnly<T>(parent, isChild, false);

            EventManagement.Awake(component, a, b, c, d);

            return component;
        }

        #endregion

        #region Create

        public static T Create<T>(Component parent = null, bool isChild = false, bool eventSystem = true)
            where T : Component, new()
        {
            T component = null;

            try
            {
                var type = typeof(T);

                lock (LockObject)
                {
                    component = (T) Pool.Dequeue(type) ?? new T();

                    component.Initialization(parent, isChild);
                }

                if (eventSystem) EventManagement.Awake(component);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            
            return component;
        }

        public static T Create<T, T1>(T1 a, Component parent = null, bool isChild = false) where T : Component, new()
        {
            var component = Create<T>(parent, isChild, false);

            EventManagement.Awake(component, a);

            return component;
        }

        public static T Create<T, T1, T2>(T1 a, T2 b, Component parent = null, bool isChild = false)
            where T : Component, new()
        {
            var component = Create<T>(parent, isChild, false);

            EventManagement.Awake(component, a, b);

            return component;
        }

        public static T Create<T, T1, T2, T3>(T1 a, T2 b, T3 c, Component parent = null, bool isChild = false)
            where T : Component, new()
        {
            var component = Create<T>(parent, isChild, false);

            EventManagement.Awake(component, a, b, c);

            return component;
        }

        public static T Create<T, T1, T2, T3, T4>(T1 a, T2 b, T3 c, T4 d, Component parent = null, bool isChild = false)
            where T : Component, new()
        {
            var component = Create<T>(parent, isChild, false);

            EventManagement.Awake(component, a, b, c, d);

            return component;
        }

        #endregion

        public static void Recycle(Component component)
        {
            if (!component.IsFromPool) return;

            lock (LockObject)
            {
                Pool.Enqueue(component.GetType(), component);
            }
        }

        public static void Clear()
        {
            lock (LockObject)
            {
                Pool.Clear();
            }
        }
    }
}