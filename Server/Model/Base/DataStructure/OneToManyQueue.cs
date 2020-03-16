using System.Collections.Generic;

namespace Sining.DataStructure
{
    public class OneToManyQueue<TKey, TValue> : Dictionary<TKey, Queue<TValue>>
    {
        private readonly int _recyclingLimit = 120;

        private readonly Queue<Queue<TValue>> _queue = new Queue<Queue<TValue>>();

        public OneToManyQueue()
        {
        }

        /// <summary>
        /// 设置最大缓存数量
        /// </summary>
        /// <param name="recyclingLimit">
        /// 1:防止数据量过大、所以超过recyclingLimit的数据还是走GC.
        /// 2:设置成0不控制数量，全部缓存</param>
        public OneToManyQueue(int recyclingLimit)
        {
            _recyclingLimit = recyclingLimit;
        }

        public bool Contains(TKey key, TValue value)
        {
            TryGetValue(key, out var list);

            return list != null && list.Contains(value);
        }

        public void Enqueue(TKey key, TValue value)
        {
            if (!TryGetValue(key, out var list))
            {
                list = Fetch();
                list.Enqueue(value);
                Add(key, list);
                return;
            }

            list.Enqueue(value);
        }

        public TValue Dequeue(TKey key)
        {
            if (!TryGetValue(key, out var list) || list.Count == 0)
            {
                return default;
            }

            var value = list.Dequeue();

            if (list.Count == 0) RemoveKey(key);

            return value;
        }

        public void RemoveKey(TKey key)
        {
            if (!Remove(key, out var list))
            {
                return;
            }

            Recycle(list);
        }
        
        private Queue<TValue> Fetch()
        {
            return _queue.Count <= 0 ? new Queue<TValue>() : _queue.Dequeue();
        }

        private void Recycle(Queue<TValue> list)
        {
            list.Clear();
            
            if (_recyclingLimit != 0 && _queue.Count > _recyclingLimit) return;

            _queue.Enqueue(list);
        }
    }
}