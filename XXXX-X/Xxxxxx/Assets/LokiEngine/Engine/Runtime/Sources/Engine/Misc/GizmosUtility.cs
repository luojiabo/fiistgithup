using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loki
{
	public class GizmosUtility
	{
		public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Gizmos.DrawRay(pos, direction);

			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
			Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
		}

		public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Gizmos.color = color;
			Gizmos.DrawRay(pos, direction);

			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
			Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
			Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
		}

		public static void DrawMeshs(Component component, bool draw, Color color, bool wire = true)
		{
			DrawMeshs(component.gameObject, draw, color);
		}

		public static void DrawMeshs(GameObject go, bool draw, Color color, bool wire = true)
		{
			if (draw)
			{
				GizmosColorUtility.Push();
				Gizmos.color = color;
				var meshFilters = go.GetComponentsInChildren<MeshFilter>();
				foreach (var mf in meshFilters)
				{
					if (mf.sharedMesh != null)
					{
						if (wire)
							Gizmos.DrawWireMesh(mf.sharedMesh, mf.transform.position, mf.transform.rotation, mf.transform.GetGlobalScale());
						else
							Gizmos.DrawMesh(mf.sharedMesh, mf.transform.position, mf.transform.rotation, mf.transform.GetGlobalScale());
					}
				}
				GizmosColorUtility.Pop();
			}
		}

		public static void DrawBounds(Transform transform, Bounds bounds, bool draw, Color color)
		{
			if (draw && transform)
			{
				GizmosColorUtility.Push();
				Gizmos.color = color;

				//Gizmos.DrawLine(new Vector3(bounds.min.x, bounds.center.y, bounds.center.z), new Vector3(bounds.max.x, bounds.center.y, bounds.center.z));
				//Gizmos.DrawLine(new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), new Vector3(bounds.center.x, bounds.max.y, bounds.center.z));
				//Gizmos.DrawLine(new Vector3(bounds.center.x, bounds.center.y, bounds.min.z), new Vector3(bounds.center.x, bounds.center.y, bounds.max.z));

				
		
				GizmosColorUtility.Pop();
			}
		}

		public static void DrawBoxCollider(BoxCollider box, bool draw, Color color)
		{
			Transform transform = box.transform;
			if (draw && transform)
			{
				GizmosColorUtility.Push();
				Gizmos.color = color;

				var bounds = box.bounds;
				Gizmos.DrawWireCube(transform.position + box.center, box.size);

				GizmosColorUtility.Pop();
			}
		}
	}
}
