using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityObject = UnityEngine.Object;

namespace Loki
{
	public class LokiEditorSelection
	{
		private static UnityObject msLastObject = null;
		private static UnityObject msLastSubAssetObject = null;
		private static UnityObject msLastModuleSettingsObject = null;

		public static UnityObject lastSubAssetObject
		{
			get
			{
				int id = EditorPrefs.GetInt("LokiEditorSelection.lastSubAssetObject");
				if (msLastSubAssetObject == null || msLastSubAssetObject.GetInstanceID() != id)
				{
					msLastSubAssetObject = null;
					if (id != 0)
					{
						msLastSubAssetObject = EditorUtility.InstanceIDToObject(id);
					}
				}
				return msLastSubAssetObject;
			}
			set
			{
				if (lastSubAssetObject != value)
				{
					if (value is UAssetObject)
					{
						UAssetObject subObject = (UAssetObject)value;
						if (subObject != null && !subObject.isSubAsset)
						{
							return;
						}
					}

					int id = value != null ? value.GetInstanceID() : 0;
					EditorPrefs.SetInt("LokiEditorSelection.lastSubAssetObject", id);
					msLastSubAssetObject = value;
				}
			}
		}

		public static UnityObject lastModuleSettingsObject
		{
			get
			{
				int id = EditorPrefs.GetInt("LokiEditorSelection.lastModuleSettingsObject");
				if (msLastModuleSettingsObject == null || msLastModuleSettingsObject.GetInstanceID() != id)
				{
					msLastModuleSettingsObject = null;
					if (id != 0)
					{
						msLastModuleSettingsObject = EditorUtility.InstanceIDToObject(id);
					}
				}
				return msLastModuleSettingsObject;
			}
			set
			{
				if (value is ModuleSettings && lastModuleSettingsObject != value)
				{
					int id = value != null ? value.GetInstanceID() : 0;
					EditorPrefs.SetInt("LokiEditorSelection.lastModuleSettingsObject", id);
					msLastModuleSettingsObject = value;
				}
			}
		}

		public static UnityObject lastSelectedObject
		{
			get
			{
				int id = EditorPrefs.GetInt("LokiEditorSelection.lastSelectedObject");
				if (msLastObject == null || msLastObject.GetInstanceID() != id)
				{
					msLastObject = null;
					if (id != 0)
					{
						msLastObject = EditorUtility.InstanceIDToObject(id);
					}
				}
				return msLastObject;
			}
			set
			{
				if (lastSelectedObject != value)
				{
					int id = value != null ? value.GetInstanceID() : 0;
					EditorPrefs.SetInt("LokiEditorSelection.lastSelectedObject", id);
					msLastObject = value;
				}

				lastSubAssetObject = value;
				lastModuleSettingsObject = value;
			}
		}
	}
}
