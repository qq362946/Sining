using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sining.Event;
using Sining.Tools;

namespace Sining
{
    /// <summary>
    /// 基础组件
    /// </summary>
    public class Component : IDisposable
    {
        private Dictionary<Type, Component> _components;
        
        private Dictionary<Type, Component> Components =>_components ??= ObjectPool<Dictionary<Type, Component>>.Fetch();
        public Component Parent { get; private set; }

        public bool IsDispose { get; private set; }
        public long InstanceId { get; private set; }

        public bool IsFromPool { get; private set; }
        
        public int AtomicLockId;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Initialization(Component parent = null, bool isFromPool = true)
        {
            parent?.AddComponent(this);

            IsDispose = false;
            InstanceId = IdFactory.NextId;
            IsFromPool = isFromPool;
        }

        public T GetParent<T>() where T : Component
        {
            return Parent as T;
        }

        #region AddComponent

        public void AddComponent(Component component)
        {
            var type = component.GetType();
            
            if (Components.ContainsKey(type))
            {
                throw new Exception($"Repeatedly added a Component of type {type.Name}");
            }
            
            component.Parent = this;
            
            Components.Add(type, component);
        }

        public T AddComponent<T>(bool isFromPool = true) where T : Component, new()
        {
            var type = typeof(T);

            if (Components.ContainsKey(type))
            {
                throw new Exception($"Repeatedly added a Component of type {type.Name}");
            }

            return isFromPool ? ComponentFactory.Create<T>(this) : ComponentFactory.CreateOnly<T>(this);
        }

        public T AddComponent<T, T1>(T1 a, bool isFromPool = true) where T : Component, new()
        {
            var type = typeof(T);

            if (Components.ContainsKey(type))
            {
                throw new Exception($"Repeatedly added a Component of type {type.Name}");
            }

            return isFromPool
                ? ComponentFactory.Create<T, T1>(a, this)
                : ComponentFactory.CreateOnly<T, T1>(a, this);
        }

        public T AddComponent<T, T1, T2>(T1 a, T2 b, bool isFromPool = true) where T : Component, new()
        {
            var type = typeof(T);

            if (Components.ContainsKey(type))
            {
                throw new Exception($"Repeatedly added a Component of type {type.Name}");
            }

            return isFromPool
                ? ComponentFactory.Create<T, T1, T2>(a, b, this)
                : ComponentFactory.CreateOnly<T, T1, T2>(a, b, this);
        }

        public T AddComponent<T, T1, T2, T3>(T1 a, T2 b, T3 c, bool isFromPool = true) where T : Component, new()
        {
            var type = typeof(T);

            if (Components.ContainsKey(type))
            {
                throw new Exception($"Repeatedly added a Component of type {type.Name}");
            }

            return isFromPool
                ? ComponentFactory.Create<T, T1, T2>(a, b, this)
                : ComponentFactory.CreateOnly<T, T1, T2>(a, b, this);
        }

        public T AddComponent<T, T1, T2, T3, T4>(T1 a, T2 b, T3 c, T4 d, bool isFromPool = true)
            where T : Component, new()
        {
            var type = typeof(T);

            if (Components.ContainsKey(type))
            {
                throw new Exception($"Repeatedly added a Component of type {type.Name}");
            }

            return isFromPool
                ? ComponentFactory.Create<T, T1, T2, T3, T4>(a, b, c, d, this)
                : ComponentFactory.CreateOnly<T, T1, T2, T3, T4>(a, b, c, d, this);
        }

        public T AddComponent<T>(Component component) where T : Component
        {
            var type = component.GetType();

            if (component.Parent != null)
            {
                throw new Exception($"Parent already exists, component type {type.Name}");
            }

            if (Components.ContainsKey(type))
            {
                throw new Exception($"Repeatedly added a Component of type {type.Name}");
            }

            Components.Add(type, component);

            component.Parent = this;

            return (T) component;
        }

        #endregion

        public T GetComponent<T>() where T : Component
        {
            if (!Components.TryGetValue(typeof(T), out var component))
            {
                return default;
            }

            return (T) component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            RemoveComponent(typeof(T));
        }

        public void RemoveComponent(Type type)
        {
            if (!Components.TryGetValue(type, out var component))
            {
                return;
            }

            Components.Remove(type);
            
            component.Dispose();
        }

        public virtual void Dispose()
        {
            if (IsDispose) return;

            EventManagement.Destroy(this);

            if (_components != null)
            {
                foreach (var childrenValue in _components.Values)
                {
                    childrenValue.Dispose();
                }

                _components.Clear();
            }

            IsFromPool = false;
            IsDispose = true;
            InstanceId = 0;

            Parent.RemoveComponent(GetType());
            ComponentFactory.Recycle(this);
        }
    }
}