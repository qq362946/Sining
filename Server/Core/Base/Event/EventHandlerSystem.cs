namespace Sining.Event
{
    public abstract class EventHandlerSystem : IEvent
    {
        protected abstract void Handler();

        public void Run()
        {
            Handler();
        }
    }

    public abstract class EventHandlerSystem<T> : IEvent<T>
    {
        protected abstract void Handler(T a);

        public void Run(T a)
        {
            Handler(a);
        }
    }

    public abstract class EventHandlerSystem<T, T1> : IEvent<T, T1>
    {
        protected abstract void Handler(T a, T1 b);

        public void Run(T a, T1 b)
        {
            Handler(a, b);
        }
    }

    public abstract class EventHandlerSystem<T, T1, T2> : IEvent<T, T1, T2>
    {
        protected abstract void Handler(T a, T1 b, T2 c);

        public void Run(T a, T1 b, T2 c)
        {
            Handler(a, b, c);
        }
    }

    public abstract class EventHandlerSystem<T, T1, T2, T3> : IEvent<T, T1, T2, T3>
    {
        protected abstract void Handler(T a, T1 b, T2 c, T3 d);

        public void Run(T a, T1 b, T2 c, T3 d)
        {
            Handler(a, b, c, d);
        }
    }

    public abstract class EventHandlerSystem<T, T1, T2, T3, T4> : IEvent<T, T1, T2, T3, T4>
    {
        protected abstract void Handler(T a, T1 b, T2 c, T3 d, T4 e);

        public void Run(T a, T1 b, T2 c, T3 d, T4 e)
        {
            Handler(a, b, c, d, e);
        }
    }
}