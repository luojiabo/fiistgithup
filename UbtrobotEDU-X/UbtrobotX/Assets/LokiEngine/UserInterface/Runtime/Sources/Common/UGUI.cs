using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Loki.UI
{
	public static class UGUI
	{
		public static RectTransform ExtendHeight(this RectTransform rectTransform, float extraHeight = 0.0f, bool recursive = false)
		{
			ProfilingUtility.BeginSample("UI-ExtendHeight");
			ExtendHeightInternal(rectTransform, extraHeight, recursive);
			ProfilingUtility.EndSample();
			return rectTransform;
		}

		private static RectTransform ExtendHeightInternal(RectTransform rectTransform, float extraHeight, bool recursive)
		{
			float sumHeight = 0.0f;
			var childCount = rectTransform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				var child = rectTransform.GetChild(i) as RectTransform;
				if (child != null && child.gameObject.activeSelf)
				{
					if (recursive)
					{
						ExtendHeightInternal(child, 0.0f, true);
					}

					sumHeight += child.sizeDelta.y;
				}
			}

			sumHeight += extraHeight;
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, sumHeight);
			return rectTransform;
		}

		public static Text SetTextResize(this Text text, string content)
		{
			text.text = content;
			text.PreferredHeight();
			return text;
		}

		public static Text PreferredHeight(this Text text)
		{
			var rectTransform = text.rectTransform;
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, text.preferredHeight);
			return text;
		}

		public static bool SetAsAnchoredPosition(this RectTransform rectTransform, Camera worldSpace, Vector3 targetPosition)
		{
			var angle = Vector3.Angle((targetPosition - worldSpace.transform.position), worldSpace.transform.forward);
			if (angle >= 90.0f && angle <= 270.0f)
			{
				return false;
			}
			var viewportPoint = worldSpace.WorldToViewportPoint(targetPosition);
			rectTransform.anchorMin = viewportPoint;
			rectTransform.anchorMax = viewportPoint;
			return true;
		}
	}
}
