using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Loki
{
	public class LokiHierarchyWindow
	{
		public static void InitializeOnLoad()
		{
			EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
		}

		//public static bool HasFocus()
		//{
		//	EditorWindow w = EditorWindow.focusedWindow;
		//	if (w == null)
		//		return false;

		//	var type = w.GetType();

		//	string t = type.ToString();
		//	if (t == "UnityEditor.SceneHierarchyWindow")
		//	{
		//		try
		//		{
		//			var sceneHierarchy = type.GetProperty("sceneHierarchy", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		//			if (sceneHierarchy == null)
		//			{
		//				return false;
		//			}
		//			var sceneHierarchyValue = sceneHierarchy.GetValue(w);
		//			if (sceneHierarchyValue == null)
		//			{
		//				return false;
		//			}
		//			var treeView = sceneHierarchy.PropertyType.GetProperty("treeView", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		//			if (treeView == null)
		//			{
		//				return false;
		//			}
		//			var treeViewValue = treeView.GetValue(sceneHierarchyValue);
		//			if (treeViewValue == null)
		//			{
		//				return false;
		//			}
		//			var hasFocus = treeView.PropertyType.GetMethod("HasFocus", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		//			if (hasFocus == null)
		//			{
		//				return false;
		//			}
		//			var hasFocusValue = hasFocus.Invoke(treeViewValue, null);
		//			if (hasFocusValue != null && hasFocusValue is bool)
		//			{
		//				return (bool)hasFocusValue;
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			DebugUtility.LogException(ex);
		//		}
		//	}
		//	return false;
		//}

		public static bool HasFocus(Rect item)
		{
			//EditorWindow w = EditorWindow.focusedWindow;
			//if (w == null)
			//	return false;

			//var type = w.GetType();

			//string t = type.ToString();
			//if (t == "UnityEditor.SceneHierarchyWindow")
			//{
				
			//}
			var e = Event.current;
			if (e == null)
				return false;

			if (item.Contains(e.mousePosition))
			{
				return true;
			}
			return false;
		}

		private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
		{
			var obj = EditorUtility.InstanceIDToObject(instanceID);
			if (obj != null)
			{
				var prefabType = PrefabUtility.GetPrefabAssetType(obj);
				if (prefabType != PrefabAssetType.NotAPrefab)
				{
					return;
				}

				if (obj is GameObject)
				{
					GameObject go = (GameObject)obj;
					var component = go.GetComponent<UObject>();
					if (component != null)
					{
						var attr = component.GetType().GetCustomAttribute<HierarchyItemAttribute>(true);
						if (attr != null)
						{
							Color fontColor;
							Color backgroundColor;
							FontStyle fontStyle = attr.fontStyle;
							if (Selection.instanceIDs.Contains(instanceID))
							{
								fontColor = attr.selectFontColor;
								backgroundColor = attr.selectBackgroundColor;
							}
							else
							{
								fontColor = attr.fontColor;
								if (HasFocus(selectionRect))
								{
									backgroundColor = new Color(0.698f, 0.698f, 0.698f);
								}
								else
								{
									backgroundColor = attr.backgroundColor;
								}
							}

							Rect offsetRect = new Rect(selectionRect.position, selectionRect.size);
							EditorGUI.DrawRect(selectionRect, backgroundColor);
							EditorGUI.LabelField(offsetRect, obj.name, new GUIStyle()
							{
								normal = new GUIStyleState() { textColor = fontColor },
								fontStyle = fontStyle
							});
						}
					}
				}

				//var prefabType = PrefabUtility.GetPrefabAssetType(obj);
				//if (prefabType == PrefabAssetType)
				//{
				//	if (Selection.instanceIDs.Contains(instanceID))
				//	{
				//		fontColor = Color.white;
				//		backgroundColor = new Color(0.24f, 0.48f, 0.90f);
				//	}

				//	Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);
				//	EditorGUI.DrawRect(selectionRect, backgroundColor);
				//	EditorGUI.LabelField(offsetRect, obj.name, new GUIStyle()
				//	{
				//		normal = new GUIStyleState() { textColor = fontColor },
				//		fontStyle = FontStyle.Bold
				//	}
				//	);
				//}
			}
		}
	}
}
