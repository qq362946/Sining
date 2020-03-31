using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Sining.Module;
using Sining.Tools;
using SqlSugar;

namespace Sining
{
    public class Component : IDisposable, IObject
    {
        #region MemberVariables
        
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0L)]
        [BsonElement]
        [BsonId]
        [JsonIgnore]
        [SugarColumn(IsNullable =false ,IsPrimaryKey =true)]
        public long Id { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        public Scene Scene { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        private Dictionary<Type, Component> ComponentsDic { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        private Dictionary<long, Component> ChildrenDic { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        private Dictionary<Type, Component> Components =>
            ComponentsDic ??= ObjectPool<Dictionary<Type, Component>>.Rent();
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        protected Dictionary<long, Component> Children => ChildrenDic ??= ObjectPool<Dictionary<long, Component>>.Rent();
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        public bool IsDispose { get; private set; }
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        public long InstanceId { get; private set; }
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        public bool IsFromPool { get; private set; }
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        private bool IsChild { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        private Component ParentComponent { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        public Component Parent
        {
            get => ParentComponent;
            set
            {
                if (value == null)
                {
                    throw new Exception($"cant set parent null: {this.GetType().Name}");
                }

                if (ParentComponent != null)
                {
                    if (ParentComponent.InstanceId == value.InstanceId)
                    {
                        throw new Exception(
                            $"Repeatedly set Parent: {GetType().Name} parent: {ParentComponent.GetType().Name}");
                    }

                    ParentComponent.RemoveChild(this);
                }

                ParentComponent = value;
                ParentComponent.AddChild(this);
                IsChild = true;
            }
        }
        [BsonElement("C")]
        [BsonIgnoreIfNull]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        private HashSet<Component> ComponentsDbHash { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        [SugarColumn(IsIgnore =true)]
        private Component ComponentParent
        {
            set => ParentComponent = value;
        }
        public T GetParent<T>() where T : Component
        {
            return ParentComponent as T;
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
            if (!ChildrenDic.TryGetValue(instanceId, out var component) || IsDispose)
            {
                return default;
            }

            return (T) component;
        }

        protected void RemoveChild(long instanceId)
        {
            if (ChildrenDic == null || IsDispose)
            {
                return;
            }

            if (!ChildrenDic.TryGetValue(instanceId, out var component))
            {
                return;
            }

            RemoveChild(component);
        }

        protected void RemoveChild(Component component)
        {
            if (ChildrenDic == null || IsDispose)
            {
                return;
            }

            if (!ChildrenDic.Remove(component.InstanceId))
            {
                return;
            }

            if (ChildrenDic.Count != 0)
            {
                return;
            }

            ObjectPool<Dictionary<long, Component>>.Return(ChildrenDic);

            ChildrenDic = null;

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
            if (ComponentsDbHash == null)
            {
                ComponentsDbHash = ObjectPool<HashSet<Component>>.Rent();
            }

            ComponentsDbHash.Add(component);
        }

        #endregion

        #region GetComponent

        public T GetComponent<T>() where T : Component
        {
            if (ComponentsDic == null || IsDispose) return default;

            Components.TryGetValue(typeof(T), out var component);

            return (T) component;
        }

        public T GetComponent<T>(Type type) where T : Component
        {
            if (ComponentsDic == null || IsDispose) return default;

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
            if (ComponentsDic == null || IsDispose) return;

            if (!Components.Remove(type, out var component)) return;

            if (ComponentsDbHash != null)
            {
                ComponentsDbHash.Remove(component);

                if (ComponentsDbHash.Count == 0 && IsFromPool)
                {
                    ObjectPool<HashSet<Component>>.Return(ComponentsDbHash);

                    ComponentsDbHash = null;
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

            if (ComponentsDic != null)
            {
                foreach (var childrenValue in ComponentsDic.Values)
                {
                    childrenValue.Dispose();
                }

                ComponentsDic.Clear();

                ObjectPool<Dictionary<Type, Component>>.Return(ComponentsDic);
                ComponentsDic = null;

                if (ComponentsDbHash != null)
                {
                    ComponentsDbHash.Clear();

                    if (IsFromPool)
                    {
                        ObjectPool<HashSet<Component>>.Return(ComponentsDbHash);
                        ComponentsDbHash = null;
                    }
                }
            }

            if (ChildrenDic != null)
            {
                foreach (var component in ChildrenDic.Values)
                {
                    component.Dispose();
                }

                ChildrenDic.Clear();

                ObjectPool<Dictionary<long, Component>>.Return(ChildrenDic);
                ChildrenDic = null;
            }

            IsDispose = true;

            if (IsChild)
            {
                ParentComponent?.RemoveChild(this);
            }
            else
            {
                ParentComponent?.RemoveComponent(this);
            }
            
            ComponentManagement.Instance.Remove(InstanceId);

            IsChild = false;
            ParentComponent = null;
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