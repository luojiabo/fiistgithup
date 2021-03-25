using System;
using System.Collections.Generic;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class LineRendererTest : UComponent
	{
		public CameraController controller;
		public Color boundColor = Color.green;

		private void Update()
		{
			if (controller == null)
				return;
			var r = controller.lineRenderer;
			r.Clear();

			var color = boundColor;

			for (int i = 0; i < transform.childCount; ++i)
			{
				if (transform.GetChild(i).GetBoundWithChildren(out var bounds))
				{
					//var color = new Color(UnityEngine.Random.Range(0.0f, 1), UnityEngine.Random.Range(0.0f, 1), UnityEngine.Random.Range(0.0f, 1), 1.0f);
					r.DrawBox(bounds, color);
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = boundColor;

			for (int i = 0; i < transform.childCount; ++i)
			{
				if (transform.GetChild(i).GetBoundWithChildren(out var bounds))
				{
					//Gizmos.color = new Color(UnityEngine.Random.Range(0.0f, 1), UnityEngine.Random.Range(0.0f, 1), UnityEngine.Random.Range(0.0f, 1), 1.0f);
					Gizmos.DrawWireCube(bounds.center, bounds.size);
				}
			}
		}
	}
}
