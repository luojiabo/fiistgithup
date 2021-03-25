using System;
using System.Collections.Generic;

namespace Loki
{
	public struct FastEnumEqualityComparer<TEnum> : IEqualityComparer<TEnum>
	{
		private readonly Func<TEnum, TEnum, bool> mEqualFunc;
		private readonly Func<TEnum, int> mToHash;

		public FastEnumEqualityComparer(Func<TEnum, TEnum, bool> equalFunc, Func<TEnum, int> toHash)
		{
			mEqualFunc = equalFunc;
			mToHash = toHash;
		}

		public bool Equals(TEnum x, TEnum y)
		{
			if (mEqualFunc != null)
			{
				return mEqualFunc(x, y);
			}
			return x.Equals(y);
		}

		public int GetHashCode(TEnum obj)
		{
			if (mToHash != null)
			{
				return mToHash(obj);
			}
			return obj.GetHashCode();
		}
	}

	public static class TEnumExtension
	{

	}
}
