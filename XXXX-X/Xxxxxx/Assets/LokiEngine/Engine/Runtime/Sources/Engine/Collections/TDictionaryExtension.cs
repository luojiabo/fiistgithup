using System;
using System.Collections.Generic;

namespace Loki
{
	public static class TDictionaryExtension
	{
		public static TValue FindOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
		{
			TValue result;
			if (!dictionary.TryGetValue(key, out result))
			{
				result = new TValue();
				dictionary.Add(key, result);
			}
			return result;
		}

		public static TResult FindOrAdd<TKey, TValue, TResult>(this Dictionary<TKey, TValue> dictionary, TKey key) where TResult : TValue, new()
		{
			TValue result;
			if (!dictionary.TryGetValue(key, out result))
			{
				TResult newValue = new TResult();
				result = newValue;
				dictionary.Add(key, result);
				return newValue;
			}

			if (!(result is TResult))
			{
				return default(TResult);
			}

			return (TResult)result;
		}
	}
}
