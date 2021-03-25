using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Loki
{
	[CustomPropertyDrawer(typeof(SceneNameAttribute))]
	public class SceneNameDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property,
												GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position,
								   SerializedProperty property,
								   GUIContent label)
		{
			var sceneNames = EditorBuildSettings.scenes.ToArray((scene) => scene.enabled ? Path.GetFileNameWithoutExtension(scene.path) : null, v => !string.IsNullOrEmpty(v));
			string currentName = property.stringValue;
			int idx = -1;
			for (int i = 0; i < sceneNames.Length; i++)
			{
				if (sceneNames[i] == currentName)
				{
					idx = i;
				}
			}
			idx = EditorGUI.Popup(position, property.displayName, idx, sceneNames);
			if (idx >= 0)
			{
				property.stringValue = sceneNames[idx];
			}
			// EditorGUI.PropertyField(position, property, label, true);
		}
	}
}
