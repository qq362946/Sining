using System;

namespace Sining.Event
{
    public interface IDestroySystem
    {
        Type Type();

        void Run(object obj);
    }

    public abstract class DestroySystem<T> : IDestroySystem
    {
        protected abstract void Destroy(T self);

        public Type Type()
        {
            return typeof(T);
        }

        public void Run(object obj)
        {
            Destroy((T) obj);
        }
    }
}