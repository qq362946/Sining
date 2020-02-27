namespace Sining.Event
{
    public interface IEvent
    {
        void Run();
    }
    
    public interface IEvent<in T>
    {
        void Run(T a);
    }

    public interface IEvent<in T, in T1>
    {
        void Run(T a, T1 b);
    }

    public interface IEvent<in T, in T1, in T2>
    {
        void Run(T a, T1 b, T2 c);
    }

    public interface IEvent<in T, in T1, in T2, in T3>
    {
        void Run(T a, T1 b, T2 c, T3 d);
    }

    public interface IEvent<in T, in T1, in T2, in T3, in T4>
    {
        void Run(T a, T1 b, T2 c, T3 d, T4 e);
    }
}