using System;
using System.Collections.Generic;
using System.Linq;

namespace Loki
{
	public class TDynamicArray<T>
	{
		private T[] mArray;
		private int mCount = 0;

		public bool fillZero { get; set; } = true;

		public int count
		{
			get
			{
				return mCount;
			}
			private set
			{
				if (mCount == value)
					return;

				mCount = value;
				if (mArray == null)
					return;

				if (fillZero)
				{
					int zeroFillCount = mArray.Length - mCount;
					if (zeroFillCount > 0)
					{
						for (int i = 0; i < zeroFillCount; ++i)
						{
							mArray[i + mCount] = default;
						}
					}
				}
			}
		}

		public T this[int index]
		{
			get
			{
				DebugUtility.AssertFormat(index >= 0 && index < count, "Please check the index range. Index : {0}, Range : [0, {1})", index, count);
				return mArray[index];
			}
		}

		/// <summary>
		/// Convert to the raw array.
		/// </summary>
		/// <param name="array">not null</param>
		public static implicit operator T[](TDynamicArray<T> array)
		{
			if (array.mArray != null)
			{
				if (array.count > 0)
				{
					T[] result = new T[array.count];
					Array.Copy(array.mArray, 0, result, 0, array.count);
					return result;
				}
			}
			return null;
		}

		/// <summary>
		/// Convert to the TDynamicArray
		/// </summary>
		/// <param name="array">not null</param>
		public static implicit operator TDynamicArray<T>(T[] array)
		{
			return new TDynamicArray<T>(array);
		}

		public TDynamicArray()
		{
			mArray = null;
			count = 0;
		}

		public TDynamicArray(int inCount)
		{
			if (inCount > 0)
			{
				mArray = new T[inCount];
			}
			count = inCount;
		}

		public TDynamicArray(T[] array)
		{
			Realloc(array);
		}

		public TDynamicArray(T[] array, int index, int length)
		{
			Realloc(array, index, length);
		}

		public TDynamicArray(IEnumerable<T> enumable)
		{
			Realloc(enumable);
		}

		public T[] Ref()
		{
			return mArray;
		}

		public void Request(int inCount, bool zeroFill)
		{
			if (mArray == null || inCount > mArray.Length)
			{
				mArray = new T[inCount];
			}
			if (zeroFill)
			{
				for (int i = 0; i < inCount; ++i)
				{
					mArray[i] = default(T);
				}
			}
			count = inCount;
		}

		public void Realloc(T[] array, int index, int length)
		{
			DebugUtility.AssertFormat(index >= 0, "The index of array must start from 0.");
			DebugUtility.AssertFormat(length >= 0 && length <= array.Length - index, "The length to copy must in the range({0}, {1}).", 0, array.Length - index);

			if (length > 0)
			{
				mArray = new T[length];
				Array.Copy(array, index, mArray, 0, mArray.Length);
			}
			count = length;
		}

		public void Realloc(IEnumerable<T> enumable)
		{
			int length = enumable.Count();
			if (length > 0)
			{
				mArray = new T[length];

				int i = 0;
				var it = enumable.GetEnumerator();
				while (it.MoveNext())
				{
					mArray[i] = it.Current;
					++i;
				}
			}
			count = length;
		}

		public void AssignFrom(T[] otherArray, int otherIndex)
		{
			AssignFrom(otherArray, otherIndex, otherArray.Length - otherIndex);
		}

		public void AssignFrom(T[] otherArray, int otherIndex, int length)
		{
			if ((mArray == null) || (length > mArray.Length))
			{
				Realloc(otherArray, otherIndex, length);
			}
			else
			{
				Array.Copy(otherArray, otherIndex, mArray, 0, length);
				count = length;
			}
		}
	}
}
