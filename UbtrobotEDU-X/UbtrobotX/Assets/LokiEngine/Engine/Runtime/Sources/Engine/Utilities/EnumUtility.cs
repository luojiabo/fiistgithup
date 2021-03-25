using System;
using System.Collections.Generic;
using System.Reflection;

namespace Loki
{
	public static class EnumUtility
	{
		public static T ToEnum<T>(this string str, T defaultValue)
		{
			T result;
			try
			{
				T t = (T)Enum.Parse(typeof(T), str);
				result = t;
			}
			catch (Exception)
			{
				result = defaultValue;
			}
			return result;
		}
	}

	public static class EnumUtility<T>
	{
		private static readonly T[] mValues;
		private static readonly string[] mNames;

		static EnumUtility()
		{
			Type type = typeof(T);
			DebugUtility.AssertFormat(type.IsEnum, "The type is not Enum.");
			Array values = Enum.GetValues(type);
			var names = Enum.GetNames(type);
			DebugUtility.AssertFormat(values.Length == names.Length, "Unknown error: The length is error.");

			mValues = new T[values.Length];
			mNames = names;
			Array.Copy(values, mValues, values.Length);
		}

		public static T[] GetValues()
		{
			return mValues;
		}

		public static string[] GetNames()
		{
			return mNames;
		}
	}

	public static class EnumUtilityNoObsoleted<T>
	{
		private static readonly T[] mValues;
		private static readonly string[] mNames;

		static EnumUtilityNoObsoleted()
		{
			Type type = typeof(T);
			DebugUtility.AssertFormat(type.IsEnum, "The type is not Enum.");
			Array values = Enum.GetValues(type);
			var names = Enum.GetNames(type);
			DebugUtility.AssertFormat(values.Length == names.Length, "Unknown error: The length is error.");

			var tValues = new List<T>(values.Length);
			var tNames = new List<string>(names.Length);

			for (int i = 0; i < names.Length; i++)
			{
				string name = names[i];
				FieldInfo info = type.GetField(name);
				if (info != null && info.GetCustomAttribute<ObsoleteAttribute>() == null)
				{
					tValues.Add((T)values.GetValue(i));
					tNames.Add(name);
				}
			}

			mValues = tValues.ToArray();
			mNames = tNames.ToArray();
		}

		public static T[] GetValues()
		{
			return mValues;
		}

		public static string[] GetNames()
		{
			return mNames;
		}
	}
}
