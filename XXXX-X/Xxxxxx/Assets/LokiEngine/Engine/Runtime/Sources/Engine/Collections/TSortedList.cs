using System;
using System.Collections.Generic;

namespace Loki
{
	/// <summary>
	/// The sorted list for high performance - foreach
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	public class TSortedList<TValue> where TValue : IEnumerable<TValue>
	{
		private readonly IComparer<TValue> mComparer;
		private readonly List<TValue> mValues;

		public TSortedList()
		{
			mValues = new List<TValue>();
			mComparer = null;
		}

		public TSortedList(IComparer<TValue> comparer)
		{
			mValues = new List<TValue>();
			mComparer = comparer;
		}

		public TSortedList(IComparer<TValue> comparer, int capacity)
		{
			mValues = new List<TValue>(capacity);
			mComparer = comparer;
		}

		public void AddRange(IEnumerable<TValue> enumerable)
		{
			foreach (var v in enumerable)
			{
				Add(v);
			}
		}

		public void Add(TValue value)
		{
			if (mComparer == null)
			{
				mValues.Add(value);
			}
			else
			{
				bool inserted = false;
				// to do optimize with binary search
				for (int i = 0; i < mValues.Count; ++i)
				{
					if (mComparer.Compare(mValues[i], value) < 0)
					{
						mValues.Insert(i, value);
						inserted = true;
						break;
					}
				}
				if (!inserted)
				{
					mValues.Add(value);
				}
			}
		}

		public List<TValue> GetSorted()
		{
			return mValues;
		}

		public List<TValue> GetSortedClone()
		{
			return new List<TValue>(mValues);
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return mValues.GetEnumerator();
		}
	}
}
