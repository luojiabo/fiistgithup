using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Loki
{
	public class BoxCameraZone : CameraZone
	{
		public Vector3 size;

		private Vector3 scaleSize
		{
			get
			{
				Vector3 value = size;
				value.Scale(transform.localScale);
				return value;
			}
		}

		private void OnDrawGizmos()
		{
			GUI.color = Color.blue;
			Gizmos.DrawWireCube(transform.position, scaleSize);
		}

		public override void Move(Transform tr, Vector3 tranlation)
		{
			var target = tr.position + tranlation;

			target.x = Mathf.Clamp(target.x, -scaleSize.x, scaleSize.x);
			target.y = Mathf.Clamp(target.y, -scaleSize.y, scaleSize.y);
			target.z = Mathf.Clamp(target.z, -scaleSize.z, scaleSize.z);

			tr.position = target;
		}
	}
}
