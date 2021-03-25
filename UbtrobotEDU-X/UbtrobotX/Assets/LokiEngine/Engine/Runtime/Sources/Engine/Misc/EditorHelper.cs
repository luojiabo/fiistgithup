#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Loki
{
	public static class EditorHelper
	{
		public static void OnSceneGUIDrawTitle(Component component, float fadeOutDistanceSqrt = 100.0f)
		{
			OnSceneGUIDrawTitle(component, Color.red, fadeOutDistanceSqrt);
		}

		public static void OnSceneGUIDrawTitle(Component component, Color color, float fadeOutDistanceSqrt = 100.0f)
		{
			if (SceneView.lastActiveSceneView == null)
				return;

			SceneView view = SceneView.lastActiveSceneView;
			if (view.camera == null)
				return;

			if (component == null)
				return;

			if ((view.camera.transform.position - component.transform.position).sqrMagnitude >= fadeOutDistanceSqrt)
				return;

			var type = component.GetType();
			var sceneDrawer = type.GetCustomAttribute<SceneDrawerAttribute>(true);
			if (sceneDrawer == null)
				return;

			string title = sceneDrawer.sceneTitle;
			if (string.IsNullOrEmpty(title))
			{
				title = string.Concat(component.name, " : ", type.Name);
			}
			//float width = HandleUtility.GetHandleSize(Vector3.zero) * 0.5f;
			var transform = component.transform;
			using (new GUIColorScope(color))
			{
				string currentTitle = title;
				string currentTooltip = sceneDrawer.tooltip;

				if (!sceneDrawer.breakCombineTooltips)
				{
					if (!string.IsNullOrEmpty(sceneDrawer.tooltip))
					{
						currentTooltip = string.Concat(title, " : ", sceneDrawer.tooltip);
					}
				}

				DynamicSceneDrawerAttribute dynamicSceneDrawer = sceneDrawer as DynamicSceneDrawerAttribute;
				if (dynamicSceneDrawer != null)
				{
					var drawerName = type.GetMethod("DynamicDrawerName", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					var drawerTooltips = type.GetMethod("DynamicDrawerTooltip", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					if (drawerName != null)
					{
						try
						{
							currentTitle = drawerName.Invoke(component, new object[] { title }) as string;
						}
						catch (Exception ex)
						{
							DebugUtility.LogException(ex);
						}
					}
					currentTitle = currentTitle ?? "(None)";
					if (drawerTooltips != null)
					{
						try
						{
							currentTooltip = drawerTooltips.Invoke(component, new object[] { title, sceneDrawer.tooltip }) as string;
						}
						catch (Exception ex)
						{
							DebugUtility.LogException(ex);
						}
					}
					currentTooltip = currentTooltip ?? "(None)";
				}
				Handles.Label(transform.position, new GUIContent(currentTitle, currentTooltip));
			}
		}
	}
}
#endif
