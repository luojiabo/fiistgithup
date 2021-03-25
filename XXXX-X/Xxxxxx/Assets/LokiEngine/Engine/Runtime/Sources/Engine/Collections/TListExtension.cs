using System;
using System.Collections.Generic;

namespace Loki
{
	public static class TListExtension
	{
		public static List<T> Union<T>(this List<T> list, T element)
		{
			if (!list.Contains(element))
				list.Add(element);
			return list;
		}

		public static int Union<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> list, TKey key, TValue value, IEqualityComparer<TKey> comparer = null)
		{
			comparer = EqualityComparer<TKey>.Default;
			var idx = list.FindIndex(kv => comparer.Equals(kv.Key, key));
			if (idx >= 0)
			{
				list[idx] = new KeyValuePair<TKey, TValue>(key, value);
				return idx;
			}
			else
			{
				list.Add(new KeyValuePair<TKey, TValue>(key, value));
				return list.Count - 1;
			}
		}

		public static TList Union<T, TList>(this TList list, T element) where TList : IList<T>
		{
			if (!list.Contains(element))
				list.Add(element);
			return list;
		}

		public static List<T> RemoveRepeating<T>(this List<T> list)
		{
			List<T> result = new List<T>(list.Count);
			foreach (var element in list)
			{
				result.Union(element);
			}
			return result;
		}

		public static TList RemoveRepeating<T, TList>(this TList list) where TList : IList<T>, new()
		{
			TList result = new TList();
			foreach (var element in list)
			{
				result.Union(element);
			}
			return result;
		}

		public static string Concat<T>(this List<T> list)
		{
			return list.Concat(null, null);
		}

		public static string Concat<T>(this List<T> list, Func<T, int, string> convertFunc)
		{
			return list.Concat(convertFunc, null);
		}

		public static string Concat<T>(this List<T> list, Func<T, int, string> convertFunc, Func<string, int, string> passFunc)
		{
			int length = list.Count;
			string[] strs = new string[length];
			for (int i = 0; i < length; i++)
			{
				T element = list[i];
				string result;
				if (convertFunc != null)
				{
					result = convertFunc(element, i);
				}
				else
				{
					result = element.ToString();
				}
				if (passFunc != null)
				{
					strs[i] = passFunc(result, i);
				}
				else
				{
					strs[i] = result;
				}
			}
			return string.Concat(strs);
		}

		public static T First<T>(this List<T> list)
		{
			if (list.Count > 0)
			{
				return list[0];
			}
			return default(T);
		}

		public static T Last<T>(this List<T> list)
		{
			if (list.Count > 0)
			{
				return list[list.Count - 1];
			}
			return default(T);
		}

		public static T First<T>(this List<T> list, T defaultValue)
		{
			if (list.Count > 0)
			{
				return list[0];
			}
			return defaultValue;
		}

		public static T Last<T>(this List<T> list, T defaultValue)
		{
			if (list.Count > 0)
			{
				return list[list.Count - 1];
			}
			return defaultValue;
		}

	}
}
