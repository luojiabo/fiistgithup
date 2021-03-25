using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Loki
{
	//[CustomEditor(typeof(CameraController), true)]

	[CanEditMultipleObjects, CustomEditor(typeof(CameraController), true)]
	public sealed class LokiCameraControllerEditor : UComponentEditor<LokiCameraControllerEditor>
	{
		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();
		}

		protected override void OnDrawCustomMethods(string category)
		{
			base.OnDrawCustomMethods(category);
		}

		public void OnSceneUICameraController(CameraController cc)
		{
			float width = HandleUtility.GetHandleSize(Vector3.zero) * 0.5f;
			var transform = cc.transform;
			var viewTarget = cc.viewTarget;

			if (viewTarget == null)
			{
				if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, 100000.0f))
				{
					Vector3 direction = transform.position - hitInfo.point;
					Handles.Label(transform.position, string.Format("{0}; (Missing View target, Distance : {1})", cc.name, direction.magnitude.ToString()));
					Handles.color = Color.green;
					Handles.DrawLine(transform.position, hitInfo.point);
				}
				else
				{
					Handles.Label(transform.position, string.Format("{0}; (Missing View target, Distance : NAN)", cc.name));
					Handles.color = Color.red;
					Handles.DrawLine(transform.position, transform.forward * 100.0f + transform.position);
				}
			}
			else
			{
				Vector3 direction = transform.position - viewTarget.position;
				Handles.Label(transform.position, string.Concat(cc.name, ": Distance(", direction.magnitude.ToString(), ")"));
				Handles.color = Color.green;
				Handles.DrawLine(transform.position, viewTarget.position);
			}
		}

		private void OnSceneGUI()
		{
			CameraController cc = (CameraController)target;
			OnSceneUICameraController(cc);
			//if (GUI.changed)
			//{
			//		EditorUtility.SetDirty(cc);
			//}
		}
	}

}
