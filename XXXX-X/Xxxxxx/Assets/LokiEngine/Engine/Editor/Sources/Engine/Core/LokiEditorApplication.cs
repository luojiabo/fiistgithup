using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;

using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace Loki
{
	public class LokiEditorApplication
	{
		private static readonly Dictionary<Type, MethodInfo> msEditorUpdates = new Dictionary<Type, MethodInfo>();
		private static readonly float DelayCollectObjects = 0.5f;
		private static double msLastCollectObjectsTime = 0.0;
		private static UnityObject[] msLastObjects = null;

		public static void InitializeOnLoad()
		{
			LokiEditorTime.Reset();
			EditorApplication.update += OnEditorUpdate;
		}

		private static void EditorUpdateInternal()
		{
			UnityObject[] result = msLastObjects;
			if ((LokiEditorTime.time - msLastCollectObjectsTime >= DelayCollectObjects) || result == null)
			{
				msLastCollectObjectsTime = LokiEditorTime.time;
				result = msLastObjects = Resources.FindObjectsOfTypeAll(typeof(UObject));
				// DebugUtility.Log(LogType.Log, "EditorApplication", "更新数据, {0}", LokiEditorTime.time.ToString());
			}
			else
			{
				// performance warning
				return;
			}

			if (result != null)
			{
				foreach (var o in result)
				{
					Type type = o.GetType();
					if (!msEditorUpdates.TryGetValue(type, out var editorUpdate))
					{
						editorUpdate = type.GetMethod("OnEditorUpdate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
						msEditorUpdates.Add(type, editorUpdate);
					}

					try
					{
						if (editorUpdate != null && !editorUpdate.IsAbstract)
						{
							editorUpdate.Invoke(o, null);
						}
					}
					catch (Exception ex)
					{
						DebugUtility.LogException(ex);
					}
				}
			}
		}
		private static void OnEditorUpdate()
		{
			LokiEditorTime.Update();

			ProfilingUtility.BeginSample("OnLokiEditorUpdate");
			EditorUpdateInternal();
			ProfilingUtility.EndSample();
		}
	}
}
