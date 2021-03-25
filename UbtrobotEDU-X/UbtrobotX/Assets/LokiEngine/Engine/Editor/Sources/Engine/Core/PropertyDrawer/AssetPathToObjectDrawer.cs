using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Loki
{
	[CustomPropertyDrawer(typeof(AssetPathToObjectAttribute))]
	public class AssetPathToObjectDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property,
												GUIContent label)
		{
			float height = EditorGUI.GetPropertyHeight(property, label, true);
			if (property.propertyType == SerializedPropertyType.String)
			{
				height *= 2.0f;
			}
			return height;
		}

		public override void OnGUI(Rect position,
								   SerializedProperty property,
								   GUIContent label)
		{
			bool enabledGUI = GUI.enabled;
			var color = GUI.color;
			bool readOnlyPath = false;
			bool readOnlyObject = false;

			if (attribute is AssetPathToObjectAttribute)
			{
				AssetPathToObjectAttribute attr = (AssetPathToObjectAttribute)attribute;
				readOnlyPath = attr.readOnlyPath;
				readOnlyObject = attr.readOnlyObject;
			}

			GUI.enabled = !readOnlyPath;
			position.height = position.height / 2.0f;
			EditorGUI.PropertyField(position, property, label, true);

			position.y += position.height;
			if (property.propertyType == SerializedPropertyType.String)
			{
				string path = property.stringValue;
				UnityEngine.Object target = null;
				if (!string.IsNullOrEmpty(path) && path.StartsWith("Assets/"))
				{
					target = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
				}
				EditorGUI.BeginChangeCheck();
				if (target == null)
				{
					GUI.color = Color.red;
				}
				GUI.enabled = !readOnlyObject;
				target = EditorGUI.ObjectField(position, target, typeof(UnityEngine.Object), false);
				GUI.color = color;
				if (EditorGUI.EndChangeCheck())
				{
					if (target != null)
					{
						property.stringValue = AssetDatabase.GetAssetPath(target);
					}
					else
					{
						property.stringValue = string.Empty;
					}
				}
			}

			GUI.enabled = enabledGUI;
			GUI.color = color;
		}
	}
}
