using System;
using System.Collections.Generic;
using System.Linq;

namespace Loki
{
	public abstract class MemoryPool : IDisposable
	{
		protected static readonly List<MemoryPool> msAllPools = new List<MemoryPool>();
		protected bool mHasDisposed = false;

		public virtual void Clear()
		{

		}

		public void Dispose()
		{
			Dispose(true);
			System.GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (mHasDisposed)
				return;

			mHasDisposed = true;
			OnDispose(disposing);
		}

		protected virtual void OnDispose(bool disposing)
		{

		}
	}

	public sealed class MemoryPool<T> : MemoryPool where T : class
	{
		private static readonly MemoryPool<T> msInstance = new MemoryPool<T>();
		private Action<T> mOnReset;
		private Func<T, T> mOnNew;

		private readonly Stack<T> mValues = new Stack<T>();

		public static MemoryPool<T> defaultInstance { get { return msInstance; } }

		public void SetNew(Func<T, T> onNew)
		{
			mOnNew = onNew;
		}

		public void SetReset(Action<T> onReset)
		{
			mOnReset = onReset;
		}

		public MemoryPool()
		{
			msAllPools.Add(this);
		}

		~MemoryPool()
		{
			Dispose(false);
		}

		public void Push(T value)
		{
			Push(value, mOnReset);
		}

		public void Push(T value, Action<T> onReset)
		{
			if (value != null)
			{
				if (onReset != null)
				{
					onReset(value);
				}
			}
			mValues.Push(value);
		}

		public T Pop()
		{
			return Pop(mOnNew);
		}

		public T Pop(Func<T, T> onNew)
		{
			T result = null;
			if (mValues.Count > 0)
			{
				result = mValues.Pop();
			}
			if (onNew != null)
				return onNew(result);
			return null;
		}

		protected override void OnDispose(bool disposing)
		{
			base.OnDispose(disposing);

			msAllPools.Remove(this);
			if (disposing)
			{
				Clear();
			}
		}

		public override void Clear()
		{
			mValues.Clear();
		}
	}
}
