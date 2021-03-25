using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public static class RectUtility
	{
		public static Rect Generate(float width, float height)
		{
			return new Rect(0, 0, width, height);
		}

		public static Rect Generate(Vector2 size)
		{
			return new Rect(Vector2.zero, size);
		}

		public static Rect Offset(this Rect rect, Vector2 offset)
		{
			rect.position += offset;
			return rect;
		}

		public static Rect Offset(this Rect rect, float offsetX, float offsetY)
		{
			rect.position += new Vector2(offsetX, offsetY);
			return rect;
		}

		public static Rect OffsetY(this Rect rect, float offsetY)
		{
			rect.position += new Vector2(0, offsetY);
			return rect;
		}

		public static Rect OffsetX(this Rect rect, float offsetX)
		{
			rect.position += new Vector2(offsetX, 0);
			return rect;
		}

		public static Rect Scale(this Rect rect, Vector2 scale)
		{
			var size = rect.size;
			size.Scale(scale);
			rect.size = size;
			return rect;
		}

		public static Rect ScaleMax(this Rect rect, Vector2 scale)
		{
			var size = rect.size;
			var oldSize = size;
			size.Scale(scale);
			rect.max += size - oldSize;
			return rect;
		}

		public static Rect ScaleMin(this Rect rect, Vector2 scale)
		{
			var size = rect.size;
			var oldSize = size;
			size.Scale(scale);
			rect.min -= size - oldSize;
			return rect;
		}
	}
}
