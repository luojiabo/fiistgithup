using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Loki
{
	public class SphereCameraZone : CameraZone
	{
		public float radius = 1.0f;
		public float yMin = 0f;

		private void OnDrawGizmos()
		{
			GUI.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, radius);
		}

		public override void Move(Transform tr, Vector3 translation)
		{
			Vector3 target = tr.position + translation;

			if (yMin != 0.0f)
			{
				target.y = Mathf.Clamp(target.y, yMin, radius);
			}

			Vector3 offset = target - transform.position;
			float offsetSqrMagnitude = offset.sqrMagnitude;
			if (offsetSqrMagnitude <= radius * radius)
			{
				tr.position = target;
			}
			else
			{
				tr.position = offset.normalized * radius;
			}
		}
	}
}
