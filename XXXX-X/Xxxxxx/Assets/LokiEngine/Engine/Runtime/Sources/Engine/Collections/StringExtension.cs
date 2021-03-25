using System.Globalization;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Loki
{
	public static class FStringExtension
	{
		public static char directorySeparatorChar { get; private set; }
		public static char altDirectorySeparatorChar { get; private set; }

		static FStringExtension()
		{
			directorySeparatorChar = Path.DirectorySeparatorChar;
			altDirectorySeparatorChar = Path.AltDirectorySeparatorChar;
		}

		public static char GetFirst(this string str)
		{
			return str[0];
		}

		public static char GetLast(this string str)
		{
			return str[str.Length - 1];
		}

		public static string ToFormatString(this string str)
		{
			string result = str;
			if (result.Contains("{"))
			{
				result = result.Replace("{", "{{");
			}
			if (result.Contains("}"))
			{
				result = result.Replace("}", "}}");
			}
			return result;
		}

		public static bool ContainsIgnoreCase(this string str, char c, CultureInfo cultureInfo)
		{
			if (string.IsNullOrEmpty(str))
				return false;

			c = char.ToLower(c, cultureInfo);

			int num = str.Length;
			for (int i = 0; i < num; ++i)
			{
				if (char.ToLower(str[i], cultureInfo) == c)
				{
					return true;
				}
			}
			return false;
		}

		public static bool Contains(this string str, char c)
		{
			if (string.IsNullOrEmpty(str))
				return false;

			int num = str.Length;
			for (int i = 0; i < num; ++i)
			{
				if (str[i] == c)
				{
					return true;
				}
			}
			return false;
		}

		public static bool ContainsAny(this string str, List<string> list)
		{
			foreach (var data in list)
			{
				if (str.Contains(data))
				{
					return true;
				}
			}
			return false;
		}

		public static string ToUNIXStyle(this string path)
		{
			if (path.Contains('\\'))
				return path.Replace('\\', '/');
			return path;
		}

		public static string ToWindowsStyle(this string path)
		{
			if (path.Contains('/'))
				return path.Replace('/', '\\');
			return path;
		}

		public static string ToPlatformStyle(this string path)
		{
			if (directorySeparatorChar == '\\')
			{
				return path.ToWindowsStyle();
			}

			return path.ToUNIXStyle();
		}

		public static bool EndsWithAnyChar(this string path, char[] symbols)
		{
			char c = path.GetLast();
			if (symbols.Contains(c))
			{
				return true;
			}
			return false;
		}

		public static bool StartsWithAnyChar(this string path, char[] symbols)
		{
			char c = path.GetFirst();
			if (symbols.Contains(c))
			{
				return true;
			}
			return false;
		}

	}
}
