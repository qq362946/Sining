using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sining.Tools
{
    public static class ObjectPool<T> where T : class, new()
    {
        private static readonly ConcurrentQueue<T> Pool = new ConcurrentQueue<T>();

        public static T Rent()
        {
            return Pool.TryDequeue(out var t) ? t : new T();
        }

        public static void Return(T t)
        {
            Pool.Enqueue(t);
        }

        public static void Clear()
        {
            Pool.Clear();
        }
    }
}