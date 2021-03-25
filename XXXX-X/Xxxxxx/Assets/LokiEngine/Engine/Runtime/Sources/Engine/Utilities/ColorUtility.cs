using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Loki
{
	public static class ColorExtensions
	{
		public static Color Alpha(this Color color, float newAlpha)
		{
			color.a = newAlpha;
			return color;
		}

		public static string ToHexRGB(this Color color)
		{
			return ColorUtility.ToHtmlStringRGB(color);
		}

		public static Color ToColor(this string htmlStringRGB, Color defaultColor)
		{
			if (!string.IsNullOrEmpty(htmlStringRGB) && ColorUtility.TryParseHtmlString(htmlStringRGB, out var result))
			{
				return result;
			}
			return defaultColor;
		}

		public static string ToHexRGBA(this Color color)
		{
			return ColorUtility.ToHtmlStringRGBA(color);
		}

		public static string ToHtmlStringRGB(this Color color)
		{
			return "#" + ColorUtility.ToHtmlStringRGB(color);
		}

		public static string ToHtmlStringRGBA(this Color color)
		{
			return "#" + ColorUtility.ToHtmlStringRGBA(color);
		}
	}
}
