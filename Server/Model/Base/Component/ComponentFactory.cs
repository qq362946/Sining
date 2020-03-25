using System;
using Sining.DataStructure;
using Sining.Event;
using Sining.Module;
using Sining.Tools;

namespace Sining
{
    public class ComponentFactory
    {
        private static readonly OneToManyQueue<Type, Component> Pool = new OneToManyQueue<Type, Component>(0);

        private static readonly object LockObject = new object();
        
        #region CreateOnly

        public static T CreateOnly<T>(Scene scene, Component parent = null, bool isChild = false,
            bool eventSystem = true)
            where T : Component, new()
        {
            T component = null;

            try
            {
                lock (LockObject)
                {
                    component = (T) Pool.Dequeue(typeof(T)) ?? new T();

                    component.Initialization(scene, parent, isChild, false);
                }

                if (eventSystem) ComponentManagement.Instance.Awake(component);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return component;
        }

        public static T CreateOnly<T, T1>(Scene scene, T1 a, Component parent = null, bool isChild = false)
            where T : Component, new()
        {
            var component = CreateOnly<T>(scene, parent, isChild, false);

            ComponentManagement.Instance.Awake(component, a);

            return component;
        }

        public static T CreateOnly<T, T1, T2>(Scene scene, T1 a, T2 b, Component parent = null, bool isChild = false)
            where T : Component, new()
        {
            var component = CreateOnly<T>(scene, parent, isChild, false);

            ComponentManagement.Instance.Awake(component, a, b);

            return component;
        }

        public static T CreateOnly<T, T1, T2, T3>(Scene scene, T1 a, T2 b, T3 c, Component parent = null,
            bool isChild = false)
            where T : Component, new()
        {
            var component = CreateOnly<T>(scene, parent, isChild, false);

            ComponentManagement.Instance.Awake(component, a, b, c);

            return component;
        }

        public static T CreateOnly<T, T1, T2, T3, T4>(Scene scene, T1 a, T2 b, T3 c, T4 d, Component parent = null,
            bool isChild = false)
            where T : Component, new()
        {
            var component = CreateOnly<T>(scene, parent, isChild, false);

            ComponentManagement.Instance.Awake(component, a, b, c, d);

            return component;
        }

        #endregion

        #region Create

        public static T Create<T>(Scene scene, Component parent = null, bool isChild = false, bool eventSystem = true)
            where T : Component, new()
        {
            T component = null;

            try
            {
                var type = typeof(T);

                lock (LockObject)
                {
                    component = (T) Pool.Dequeue(type) ?? new T();

                    component.Initialization(scene, parent, isChild);
                }

                if (eventSystem) ComponentManagement.Instance.Awake(component);

                ComponentManagement.Instance.Register(component);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return component;
        }
        public static T Create<T, T1>(Scene scene, T1 a, Component parent = null, bool isChild = false)
            where T : Component, new()
        {
            var component = Create<T>(scene, parent, isChild, false);

            ComponentManagement.Instance.Awake(component, a);

            return component;
        }
        public static T Create<T, T1, T2>(Scene scene, T1 a, T2 b, Component parent = null, bool isChild = false)
            where T : Component, new()
        {
            var component = Create<T>(scene, parent, isChild, false);

            ComponentManagement.Instance.Awake(component, a, b);

            return component;
        }
        public static T Create<T, T1, T2, T3>(Scene scene, T1 a, T2 b, T3 c, Component parent = null,
            bool isChild = false)
            where T : Component, new()
        {
            var component = Create<T>(scene, parent, isChild, false);

            ComponentManagement.Instance.Awake(component, a, b, c);

            return component;
        }
        public static T Create<T, T1, T2, T3, T4>(Scene scene, T1 a, T2 b, T3 c, T4 d, Component parent = null,
            bool isChild = false)
            where T : Component, new()
        {
            var component = Create<T>(scene, parent, isChild, false);

            ComponentManagement.Instance.Awake(component, a, b, c, d);

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