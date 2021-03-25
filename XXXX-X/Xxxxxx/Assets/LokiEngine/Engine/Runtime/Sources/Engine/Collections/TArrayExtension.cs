using System.Collections.Generic;
using System;

namespace Loki
{
	public static class TArrayExtension
	{
		public static bool Contains<T>(this T[] array, T element) where T : IEquatable<T>
		{
			foreach (var a in array)
			{
				if (a.Equals(element))
					return true;
			}
			return false;
		}

		public static bool Contains<T>(this T[] array, T element, IEqualityComparer<T> comparer)
		{
			foreach (var a in array)
			{
				if (comparer.Equals(a, element))
					return true;
			}
			return false;
		}

		public static int ToArray(this string[] sources, out int[] array)
		{
			array = new int[sources.Length];
			for (var i = 0; i < sources.Length; ++i)
			{
				int.TryParse(sources[i], out var result);
				array[i] = result;
			}
			return array.Length;
		}

		public static int ToArray(this string[] sources, out long[] array)
		{
			array = new long[sources.Length];
			for (var i = 0; i < sources.Length; ++i)
			{
				long.TryParse(sources[i], out var result);
				array[i] = result;
			}
			return array.Length;
		}

		public static int ToArray(this string[] sources, out bool[] array)
		{
			array = new bool[sources.Length];
			for (var i = 0; i < sources.Length; ++i)
			{
				bool.TryParse(sources[i], out var result);
				array[i] = result;
			}
			return array.Length;
		}

		public static int ToArray(this string[] sources, out uint[] array)
		{
			array = new uint[sources.Length];
			for (var i = 0; i < sources.Length; ++i)
			{
				uint.TryParse(sources[i], out var result);
				array[i] = result;
			}
			return array.Length;
		}

		public static int ToArray(this string[] sources, out float[] array)
		{
			array = new float[sources.Length];
			for (var i = 0; i < sources.Length; ++i)
			{
				float.TryParse(sources[i], out var result);
				array[i] = result;
			}
			return array.Length;
		}

		public static int ToArray(this string[] sources, out double[] array)
		{
			array = new double[sources.Length];
			for (var i = 0; i < sources.Length; ++i)
			{
				double.TryParse(sources[i], out var result);
				array[i] = result;
			}
			return array.Length;
		}

		public static int ToArray(this string[] sources, out UInt16[] array)
		{
			array = new UInt16[sources.Length];
			for (var i = 0; i < sources.Length; ++i)
			{
				UInt16.TryParse(sources[i], out var result);
				array[i] = result;
			}
			return array.Length;
		}

		public static int ToArray(this string[] sources, out byte[] array)
		{
			array = new byte[sources.Length];
			for (var i = 0; i < sources.Length; ++i)
			{
				byte.TryParse(sources[i], out var result);
				array[i] = result;
			}
			return array.Length;
		}

		public static int ToArray(this string[] sources, out UInt64[] array)
		{
			array = new UInt64[sources.Length];
			for (var i = 0; i < sources.Length; ++i)
			{
				UInt64.TryParse(sources[i], out var result);
				array[i] = result;
			}
			return array.Length;
		}

		public static TTarget[] ToArray<TSource, TTarget>(this TSource[] sources, Func<TSource, TTarget> convert, Func<TTarget, bool> validate = null)
		{
			if (validate != null)
			{
				var array = new List<TTarget>(sources.Length);
				for (var i = 0; i < sources.Length; ++i)
				{
					var result = convert(sources[i]);
					if (validate(result))
					{
						array.Add(result);
					}
				}
				return array.ToArray();
			}
			else
			{
				var array = new TTarget[sources.Length];
				for (var i = 0; i < sources.Length; ++i)
				{
					array[i] = convert(sources[i]);
				}
				return array;
			}
		}

	}
}
