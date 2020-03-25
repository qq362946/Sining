using System;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using Sining.Event;
using Sining.Module;
using Sining.Tools;

namespace Sining
{
    public class Component : IDisposable, IObject
    {
        #region MemberVariables
        
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0L)]
        [BsonElement]
        [BsonId]
        public long Id { get; set; }
        [BsonIgnore]
        public Scene Scene;
        [BsonIgnore]
        private Dictionary<Type, Component> _components;
        [BsonIgnore]
        private Dictionary<long, Component> _children;
        [BsonIgnore]
        private Dictionary<Type, Component> Components =>
            _components ??= ObjectPool<Dictionary<Type, Component>>.Rent();
        [BsonIgnore]
        protected Dictionary<long, Component> Children => _children ??= ObjectPool<Dictionary<long, Component>>.Rent();
        [BsonIgnore]
        public bool IsDispose { get; private set; }
        [BsonIgnore]
        public long InstanceId { get; private set; }
        [BsonIgnore]
        public bool IsFromPool { get; private set; }
        [BsonIgnore]
        private bool _isChild;
        [BsonIgnore]
        private Component _parent;
        [BsonIgnore]
        public Component Parent
        {
            get => _parent;
            set
            {
                if (value == null)
                {
                    throw new Exception($"cant set parent null: {this.GetType().Name}");
                }

                if (_parent != null)
                {
                    if (_parent.InstanceId == value.InstanceId)
                    {
                        throw new Exception(
                            $"Repeatedly set Parent: {GetType().Name} parent: {_parent.GetType().Name}");
                    }

                    _parent.RemoveChild(this);
                }

                _parent = value;
                _parent.AddChild(this);
                _isChild = true;
            }
        }
        [BsonElement("C")]
        [BsonIgnoreIfNull]
        private HashSet<Component> _componentsDb;
        [BsonIgnore]
        private Component ComponentParent
        {
            set => _parent = value;
        }
        public T GetParent<T>() where T : Component
        {
            return _parent as T;
        }
        
        #endregion

        #region Child

        protected void AddChild(Component component)
        {
            if (IsDispose) return;

            Children.Add(component.InstanceId, component);
        }

        protected T GetChild<T>(long instanceId) where T : Component
        {
            if (!_children.TryGetValue(instanceId, out var component) || IsDispose)
            {
                return default;
            }

            return (T) component;
        }

        protected void RemoveChild(long instanceId)
        {
            if (_children == null || IsDispose)
            {
                return;
            }

            if (!_children.TryGetValue(instanceId, out var component))
            {
                return;
            }

            RemoveChild(component);
        }

        protected void RemoveChild(Component component)
        {
            if (_children == null || IsDispose)
            {
                return;
            }

            if (!_children.Remove(component.InstanceId))
            {
                return;
            }

            if (_children.Count != 0)
            {
                return;
            }

            ObjectPool<Dictionary<long, Component>>.Return(_children);

            _children = null;

            if (!IsDispose) component.Dispose();
        }

        #endregion

        #region AddComponent

        public Component AddComponent(Component component)
        {
            if (IsDispose)
            {
                throw new Exception($"Component name {GetType().Name} has been destroyed");
            }

            var type = component.GetType();

            if (Components.ContainsKey(type))
            {
                throw new Exception($"A component of type {GetType().Name} already exists");
            }

            Components.Add(type, component);
            AddToComponentsDb(component);

            return component;
        }
        public T AddComponent<T>(bool isFromPool = true) where T : Component, new()
        {
            if (IsDispose)
            {
                throw new Exception($"Component name {GetType().Name} has been destroyed");
            }

            var type = typeof(T);

            if (Components.ContainsKey(type))
            {
                throw new Exception($"A component of type {GetType().Name} already exists");
            }

            var component = isFromPool
                ? ComponentFactory.Create<T>(Scene, this)
                : ComponentFactory.CreateOnly<T>(Scene, this);

            Components.Add(type, component);
            AddToComponentsDb(component);

            return component;
        }
        public T AddComponent<T, T1>(T1 a, bool isFromPool = true) where T : Component, new()
        {
            if (IsDispose)
            {
                throw new Exception($"Component name {GetType().Name} has been destroyed");
            }

            var type = typeof(T);

            if (Components.ContainsKey(type))
            {
                throw new Exception($"A component of type {GetType().Name} already exists");
            }

            var component = isFromPool
                ? ComponentFactory.Create<T, T1>(Scene, a, this)
                : ComponentFactory.CreateOnly<T, T1>(Scene, a, this);

            Components.Add(type, component);
            AddToComponentsDb(component);

            return component;
        }
        public T AddComponent<T, T1, T2>(T1 a, T2 b, bool isFromPool = true) where T : Component, new()
        {
            if (IsDispose)
            {
                throw new Exception($"Component name {GetType().Name} has been destroyed");
            }

            var type = typeof(T);

            if (Components.ContainsKey(type))
            {
                throw new Exception($"A component of type {GetType().Name} already exists");
            }

            var component = isFromPool
                ? ComponentFactory.Create<T, T1, T2>(Scene, a, b, this)
                : ComponentFactory.CreateOnly<T, T1, T2>(Scene, a, b, this);

            Components.Add(type, component);
            AddToComponentsDb(component);

            return component;
        }
        public T AddComponent<T, T1, T2, T3>(T1 a, T2 b, T3 c, bool isFromPool = true) where T : Component, new()
        {
            if (IsDispose)
            {
                throw new Exception($"Component name {GetType().Name} has been destroyed");
            }

            var type = typeof(T);

            if (Components.ContainsKey(type))
            {
                throw new Exception($"A component of type {GetType().Name} already exists");
            }

            var component = isFromPool
                ? ComponentFactory.Create<T, T1, T2, T3>(Scene, a, b, c, this)
                : ComponentFactory.CreateOnly<T, T1, T2, T3>(Scene, a, b, c, this);

            Components.Add(type, component);
            AddToComponentsDb(component);

            return component;
        }
        public T AddComponent<T, T1, T2, T3, T4>(T1 a, T2 b, T3 c, T4 d, bool isFromPool = true)
            where T : Component, new()
        {
            if (IsDispose)
            {
                throw new Exception($"Component name {GetType().Name} has been destroyed");
            }

            var type = typeof(T);

            if (Components.ContainsKey(type))
            {
                throw new Exception($"A component of type {GetType().Name} already exists");
            }

            var component = isFromPool
                ? ComponentFactory.Create<T, T1, T2, T3, T4>(Scene, a, b, c, d, this)
                : ComponentFactory.CreateOnly<T, T1, T2, T3, T4>(Scene, a, b, c, d, this);

            Components.Add(type, component);
            AddToComponentsDb(component);

            return component;
        }
        private void AddToComponentsDb(Component component)
        {
            if (_componentsDb == null)
            {
                _componentsDb = ObjectPool<HashSet<Component>>.Rent();
            }

            _componentsDb.Add(component);
        }

        #endregion

        #region GetComponent

        public T GetComponent<T>() where T : Component
        {
            if (_components == null || IsDispose) return default;

            Components.TryGetValue(typeof(T), out var component);

            return (T) component;
        }

        public T GetComponent<T>(Type type) where T : Component
        {
            if (_components == null || IsDispose) return default;

            Components.TryGetValue(type, out var component);

            return (T) component;
        }

        #endregion

        #region RemoveComponent

        public void RemoveComponent<T>() where T : Component
        {
            RemoveComponent(typeof(T));
        }
        public void RemoveComponent(Type type)
        {
            if (_components == null || IsDispose) return;

            if (!Components.Remove(type, out var component)) return;

            if (_componentsDb != null)
            {
                _componentsDb.Remove(component);

                if (_componentsDb.Count == 0 && IsFromPool)
                {
                    ObjectPool<HashSet<Component>>.Return(_componentsDb);

                    _componentsDb = null;
                }
            }

            component.Dispose();
        }
        public void RemoveComponent(Component component)
        {
            if (component.IsDispose) return;

            RemoveComponent(component.GetType());
        }
        
        #endregion

        #region Dispose

        public virtual void Dispose()
        {
            if (IsDispose) return;

            ComponentManagement.Instance.Destroy(this);

            if (_components != null)
            {
                foreach (var childrenValue in _components.Values)
                {
                    childrenValue.Dispose();
                }

                _components.Clear();

                ObjectPool<Dictionary<Type, Component>>.Return(_components);
                _components = null;

                if (_componentsDb != null)
                {
                    _componentsDb.Clear();

                    if (IsFromPool)
                    {
                        ObjectPool<HashSet<Component>>.Return(_componentsDb);
                        _componentsDb = null;
                    }
                }
            }

            if (_children != null)
            {
                foreach (var component in _children.Values)
                {
                    component.Dispose();
                }

                _children.Clear();

                ObjectPool<Dictionary<long, Component>>.Return(_children);
                _children = null;
            }

            IsDispose = true;

            if (_isChild)
            {
                _parent?.RemoveChild(this);
            }
            else
            {
                _parent?.RemoveComponent(this);
            }
            
            ComponentManagement.Instance.Remove(InstanceId);

            _isChild = false;
            _parent = null;
            IsFromPool = false;
            InstanceId = 0;
            Id = 0;
            Scene = null;

            ComponentFactory.Recycle(this);
        }

        #endregion

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Initialization(Scene scene, Component parent = null, bool isChild = false, bool isFromPool = true)
        {
            Scene = scene;
            IsDispose = false;
            InstanceId = IdFactory.NextId;
            Id = InstanceId;
            IsFromPool = isFromPool;

            if (parent == null) return;

            if (isChild)
            {
                Parent = parent;
            }
            else
            {
                ComponentParent = parent;
            }
        }
    }
}