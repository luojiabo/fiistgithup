using System;
using System.Collections.Generic;

namespace Loki
{
	public static class TypeUtility
	{
		private static readonly Dictionary<Type, Func<string, Type, object>> msConverters = new Dictionary<Type, Func<string, Type, object>>();

		static TypeUtility()
		{
			Register(typeof(string), (str, type) =>
			{
				return str;
			});
			Register(typeof(int), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				int.TryParse(str, out var result);
				return result;
			});
			Register(typeof(long), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				long.TryParse(str, out var result);
				return result;
			});
			Register(typeof(uint), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				uint.TryParse(str, out var result);
				return result;
			});
			Register(typeof(char), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				char.TryParse(str, out var result);
				return result;
			});
			Register(typeof(Enum), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				return Enum.Parse(type, str);
			});
			Register(typeof(UInt64), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				UInt64.TryParse(str, out var result);
				return result;
			});
			Register(typeof(UInt32), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				UInt32.TryParse(str, out var result);
				return result;
			});
			Register(typeof(UInt16), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				UInt16.TryParse(str, out var result);
				return result;
			});
			Register(typeof(Int16), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				Int16.TryParse(str, out var result);
				return result;
			});
			Register(typeof(byte), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				byte.TryParse(str, out var result);
				return result;
			});
			Register(typeof(bool), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				bool.TryParse(str, out var result);
				return result;
			});
			Register(typeof(SByte), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				SByte.TryParse(str, out var result);
				return result;
			});
			Register(typeof(float), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				float.TryParse(str, out var result);
				return result;
			});
			Register(typeof(double), (str, type) =>
			{
				if (string.IsNullOrEmpty(str))
					return 0;
				double.TryParse(str, out var result);
				return result;
			});
		}

		public static void Register(Type type, Func<string, Type, object> converter)
		{
			msConverters[type] = converter;
		}

		public static object ToObject(string valueString, Type targetType)
		{
			if (msConverters.TryGetValue(targetType, out var converted))
			{
				return converted(valueString, targetType);
			}
			return null;
		}

		public static T ToObject<T>(string valueString, Type targetType)
		{
			if (msConverters.TryGetValue(targetType, out var converted))
			{
				return (T)converted(valueString, targetType);
			}
			return default(T);
		}
	}
}
