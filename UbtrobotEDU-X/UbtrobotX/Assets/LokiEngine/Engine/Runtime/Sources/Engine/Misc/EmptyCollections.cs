using System;
using System.Collections.Generic;

namespace Loki
{
	public class ArrayHelper<T>
	{
		public static readonly T[] Empty = new T[0];

		public static T[] New(int length)
		{
			if (length <= 0)
				return Empty;

			return new T[length];
		}

		public static T[] New(int length, T fillValue)
		{
			if (length <= 0)
				return Empty;

			var result = new T[length];
			for (int i = 0; i < length; ++i)
			{
				result[i] = fillValue;
			}
			return result;
		}

		public static T[] New(int length, Func<T> fillAction)
		{
			if (length <= 0)
				return Empty;

			var result = new T[length];
			for (int i = 0; i < length; ++i)
			{
				result[i] = fillAction();
			}
			return result;
		}

		public static T[] New(int length, Func<int, T> fillAction)
		{
			if (length <= 0)
				return Empty;

			var result = new T[length];
			for (int i = 0; i < length; ++i)
			{
				result[i] = fillAction(i);
			}
			return result;
		}
	}

}
