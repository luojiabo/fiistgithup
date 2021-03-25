using System;
using System.Collections.Generic;

namespace Loki
{
	public static class HashCodeCombiner
	{
		public static int CombineHashCodes(int h1, int h2)
		{
			return ((h1 << 5) + h1) ^ h2;
		}

		public static int CombineHashCodes(int h1, int h2, int h3)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2), h3);
		}

		public static int CombineHashCodes(int h1, int h2, int h3, int h4)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4));
		}

		public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
		}

		public static int CombineHashCodes<T>(int h1, T h2)
		{
			return ((h1 << 5) + h1) ^ h2.GetHashCode();
		}

		public static int CombineHashCodes<T>(T h1, T h2)
		{
			int h1c = h1.GetHashCode();
			return ((h1c << 5) + h1c) ^ h2.GetHashCode();
		}

		public static int CombineHashCodes<T>(T h1, T h2, T h3)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2), h3);
		}

		public static int CombineHashCodes<T>(T h1, T h2, T h3, T h4)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4));
		}

		public static int CombineHashCodes<T>(T h1, T h2, T h3, T h4, T h5)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
		}


	}
}
