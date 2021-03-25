using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class HierarchyItemAttribute : LokiAttribute
	{
		/// <summary>
		/// The color is html format: #RRGGBBAA / #RRGGBB 
		/// </summary>
		public string fontColorString = string.Empty;
		/// <summary>
		/// The color is html format: #RRGGBBAA / #RRGGBB 
		/// </summary>
		public string backgroundColorString = string.Empty;
		/// <summary>
		/// The color is html format: #RRGGBBAA / #RRGGBB 
		/// </summary>
		public string selectFontColorString = string.Empty;
		/// <summary>
		/// The color is html format: #RRGGBBAA / #RRGGBB 
		/// </summary>
		public string selectBackgroundColorString = string.Empty;

		public static readonly Color DefaultBackgroundColor = new Color(194.0f / 255, 194.0f / 255, 194.0f / 255);
		public static readonly Color DefaultSelectBackgroundColor = new Color(62.0f / 255, 125.0f / 255, 231.0f / 255);

		private Color? mFontColor;
		private Color? mBackgroundFontColor;

		private Color? mSelectFontColor;
		private Color? mSelectBackgroundFontColor;

		public Color fontColor
		{
			get
			{
				return (Color)(mFontColor ?? (mFontColor = fontColorString.ToColor(Color.white)));
			}
		}

		public Color backgroundColor
		{
			get
			{
				return (Color)(mBackgroundFontColor ?? (mBackgroundFontColor = backgroundColorString.ToColor(DefaultBackgroundColor)));
			}
		}

		public Color selectFontColor
		{
			get
			{
				return (Color)(mSelectFontColor ?? (mSelectFontColor = selectFontColorString.ToColor(fontColor)));
			}
		}

		public Color selectBackgroundColor
		{
			get
			{
				return (Color)(mSelectBackgroundFontColor ?? (mSelectBackgroundFontColor = selectBackgroundColorString.ToColor(DefaultSelectBackgroundColor)));
			}
		}

		public FontStyle fontStyle { get; set; } = FontStyle.Normal;
	}

	public class LokiTooltipAttribute : LokiAttribute
	{
		public readonly string tooltip;

		public LokiTooltipAttribute(string tooltip)
		{
			this.tooltip = tooltip;
		}
	}
}
