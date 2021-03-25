using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Loki
{
	public class LokiSceneUtility
	{
		public static void DrawArrowCap(Transform transform, float arrowSize = 1.0f)
		{
			Handles.color = Handles.xAxisColor;
			Handles.ArrowHandleCap(0, transform.position, transform.rotation * Quaternion.Euler(0, 90, 0), arrowSize, EventType.Repaint);

			Handles.color = Handles.yAxisColor;
			Handles.ArrowHandleCap(0, transform.position, transform.rotation * Quaternion.Euler(-90, 0, 0), arrowSize, EventType.Repaint);

			Handles.color = Handles.zAxisColor;
			Handles.ArrowHandleCap(0, transform.position, transform.rotation, arrowSize, EventType.Repaint);
		}
	}
}
