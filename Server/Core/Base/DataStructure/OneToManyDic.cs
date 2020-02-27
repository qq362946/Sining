using System.Collections.Generic;
using System.Linq;

namespace Sining.DataStructure
{
    public class OneToManyDic<TKey, TValueKey, TValue> : Dictionary<TKey, Dictionary<TValueKey, TValue>>
    {
        private readonly int _recyclingLimit = 120;

        private readonly Queue<Dictionary<TValueKey, TValue>> _queue = new Queue<Dictionary<TValueKey, TValue>>();

        public OneToManyDic() { }

        /// <summary>
        /// 设置最大缓存数量
        /// </summary>
        /// <param name="recyclingLimit">
        /// 1:防止数据量过大、所以超过recyclingLimit的数据还是走GC.
        /// 2:设置成0不控制数量，全部缓存</param>
        public OneToManyDic(int recyclingLimit)
        {
            _recyclingLimit = recyclingLimit;
        }

        public bool Contains(TKey key, TValueKey valueKey)
        {
            TryGetValue(key, out var dic);

            return dic != null && dic.ContainsKey(valueKey);
        }

        public TValue TryGetValue(TKey key, TValueKey valueKey)
        {
            if (!TryGetValue(key, out var dic))
            {
                return default;
            }

            dic.TryGetValue(valueKey, out var value);

            return value;
        }

        public TValue First(TKey key)
        {
            return !TryGetValue(key, out var dic) ? default : dic.First().Value;
        }

        public void Add(TKey key, TValueKey valueKey, TValue value)
        {
            if (!TryGetValue(key, out var dic))
            {
                dic = Fetch();
                dic.Add(valueKey, value);
                Add(key, dic);

                return;
            }

            dic.Add(valueKey, value);
        }

        public bool RemoveByValueKey(TKey key, TValueKey valueKey,out TValue value)
        {
            if (!TryGetValue(key, out var dic))
            {
                value = default;
                return false;
            }

            var result = dic.Remove(valueKey, out value);

            if (dic.Count == 0) RemoveKey(key);

            return result;
        }

        public void RemoveKey(TKey key)
        {
            if (!Remove(key, out var dic))
            {
                return;
            }

            Recycle(dic);
        }

        private Dictionary<TValueKey, TValue> Fetch()
        {
            return _queue.Count <= 0 ? new Dictionary<TValueKey, TValue>() : _queue.Dequeue();
        }

        private void Recycle(Dictionary<TValueKey, TValue> dic)
        {
            dic.Clear();

            if (_recyclingLimit != 0 && _queue.Count > _recyclingLimit) return;

            _queue.Enqueue(dic);
        }
    }
}