using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loki
{
	public class TSafeForeachList<T>
	{
		private readonly List<T> mDatas;
		private readonly List<T> mPendingRemove = new List<T>();
		private bool mIsTransiting = false;
		private int mLastCount = -1;

		public bool transiting
		{
			get
			{
				return mIsTransiting;
			}
			set
			{
				mIsTransiting = value;
				if (mIsTransiting)
				{
					mLastCount = realtimeCount;
				}
			}
		}

		/// <summary>
		/// Use for the looping
		/// </summary>
		public int loopingCount
		{
			get
			{
				if (transiting)
				{
					return mLastCount;
				}
				return realtimeCount;
			}
		}

		public int realtimeCount
		{
			get
			{
				return mDatas.Count;
			}
		}

		public T this[int index]
		{
			get
			{
				return mDatas[index];
			}
		}

		public TSafeForeachList()
		{
			mDatas = new List<T>();
		}

		public TSafeForeachList(int capacity)
		{
			mDatas = new List<T>(capacity);
		}

		public void RemoveAll(Predicate<T> match)
		{
			DebugUtility.AssertFormat(!transiting, "Please do not call RemoveAll in transiting.");
			mDatas.RemoveAll(match);
		}

		public bool IsPendingRemove(T elementInContainer)
		{
			return mPendingRemove.Contains(elementInContainer);
		}

		public bool Contains(T elementInContainer)
		{
			return mDatas.Contains(elementInContainer);
		}

		public bool BeginForeach()
		{
			if (transiting)
				return false;
			transiting = true;
			return true;
		}

		public void EndForeach()
		{
			transiting = false;
		}

		public void Union(T element)
		{
			Add(element, true);
		}

		public void Add(T element)
		{
			Add(element, false);
		}

		private void Add(T element, bool unique)
		{
			if (transiting)
			{
				// remove from pending remove container
				if (mPendingRemove.Remove(element))
				{
					// If success to remove, it means that this element is in the container.
					return;
				}
			}

			if (unique && Contains(element))
			{
				// If this element is in the container, skip adding.
				return;
			}

			// Add it to the container directly.
			mDatas.Add(element);
		}

		public void Remove(T element)
		{
			if (transiting)
			{
				// It's already in the pending remove container, skip removing
				if (IsPendingRemove(element))
				{
					return;
				}
				// It's in the container, and it's not in the pending remove container, add to pending container
				if (Contains(element))
				{
					mPendingRemove.Add(element);
					return;
				}
				else // can't find this element in container, skip removing
				{

				}
			}
			else // It's not in the transiting, remove it directly.
			{
				mDatas.Remove(element);
			}
		}
	}

}
