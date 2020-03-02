using System;

namespace Sining.Event
{
	public interface IAwakeSystem
	{
		Type Type();
	}

	public abstract class AwakeSystem<T> : IAwakeSystem, IEvent<T>
	{
		protected abstract void Awake(T self);

		public Type Type()
		{
			return typeof(T);
		}

		public void Run(T t)
		{
			Awake(t);
		}
	}

	public abstract class AwakeSystem<T, T1> : IAwakeSystem, IEvent<T, T1>
	{
		protected abstract void Awake(T self, T1 a);

		public Type Type()
		{
			return typeof(T);
		}

		public void Run(T t, T1 a)
		{
			Awake(t, a);
		}
	}

	public abstract class AwakeSystem<T, T1, T2> : IAwakeSystem, IEvent<T, T1, T2>
	{
		protected abstract void Awake(T self, T1 a, T2 b);

		public Type Type()
		{
			return typeof(T);
		}

		public void Run(T t, T1 a, T2 b)
		{
			Awake(t, a, b);
		}
	}

	public abstract class AwakeSystem<T, T1, T2, T3> : IAwakeSystem, IEvent<T, T1, T2, T3>
	{
		protected abstract void Awake(T self, T1 a, T2 b, T3 c);

		public Type Type()
		{
			return typeof(T);
		}

		public void Run(T t, T1 a, T2 b, T3 c)
		{
			Awake(t, a, b, c);
		}
	}

	public abstract class AwakeSystem<T, T1, T2, T3, T4> : IAwakeSystem, IEvent<T, T1, T2, T3, T4>
	{
		protected abstract void Awake(T self, T1 a, T2 b, T3 c, T4 d);

		public Type Type()
		{
			return typeof(T);
		}

		public void Run(T t, T1 a, T2 b, T3 c, T4 d)
		{
			Awake(t, a, b, c, d);
		}
	}
}