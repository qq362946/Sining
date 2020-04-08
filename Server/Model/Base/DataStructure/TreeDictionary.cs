using System;
using System.Collections.Generic;
using Sining.Tools;

namespace Sining.DataStructure
{
    public class DictionaryNode<TKey, TValue> : HashSet<TKey>
    {
        public TKey Key;
        public TValue Value;
        public DictionaryNode<TKey, TValue> Parent;
        private TreeDictionary<TKey, TValue> _container;
        public DictionaryNode<TKey, TValue> Init(
            TKey key,
            TValue value,
            TreeDictionary<TKey, TValue> container,
            DictionaryNode<TKey, TValue> parent = null)
        {
            Key = key;
            Value = value;
            Parent = parent;
            _container = container;
            return this;
        }
        public new void Remove(TKey key) { }
        public bool Remove(TKey key, out TValue value)
        {
            value = default;

            if (!Contains(key)) return false;
            if (!_container.Remove(key, out var node)) return false;

            value = node.Value;
            node.Clear();
            base.Remove(key);

            ObjectPool<DictionaryNode<TKey, TValue>>.Return(node);
            return true;
        }
        public new void Clear()
        {
            foreach (var valueNode in this)
            {
                if (!_container.Remove(valueNode, out var node)) continue;

                node.Clear();
            }

            base.Clear();
        }
    }
    public class TreeDictionary<TKey, TValue> : Dictionary<TKey, DictionaryNode<TKey, TValue>>
    {
        public DictionaryNode<TKey, TValue> Add(TKey key, TValue value, TKey parentKey)
        {
            TryGetValue(parentKey, out var parent);

            return Add(key, value, parent);
        }
        public DictionaryNode<TKey, TValue> Add(TKey key, TValue value, DictionaryNode<TKey, TValue> parent = null)
        {
            DictionaryNode<TKey, TValue> node = null;
            try
            {
                node = ObjectPool<DictionaryNode<TKey, TValue>>.Rent().Init(key, value, this, parent);
                Add(key, node);
                parent?.Add(key);
            }
            catch (Exception e)
            {
                Log.Error(e);
                ObjectPool<DictionaryNode<TKey, TValue>>.Return(node);
                node = null;
            }

            return node;
        }
        public new void Remove(TKey key)
        {
            if (!TryGetValue(key, out var node)) return;

            node.Clear();
            ObjectPool<DictionaryNode<TKey, TValue>>.Return(node);
        }
        public new  void Clear()
        {
            foreach (var node in Values)
            {
                ObjectPool<DictionaryNode<TKey, TValue>>.Return(node);
            }
            
            base.Clear();
        }
    }
}