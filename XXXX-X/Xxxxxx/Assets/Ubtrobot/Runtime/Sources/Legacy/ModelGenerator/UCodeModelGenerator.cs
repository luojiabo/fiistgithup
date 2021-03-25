#if UNITY_EDITOR
#define UBTROBOT
#endif

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using LitJson;
using UnityEngine;
using UnityObject = UnityEngine.Object;
#if UBTROBOT
using Loki;
#endif

namespace Ubtrobot
{
	public class UCodeModelGenerator : MonoBehaviour
	{
		[SerializeField]
		private string mPartName;

		private void Reset()
		{
			mPartName = name;
		}

#if UNITY_EDITOR

		private static Vector3 GetGlobalScale(Transform tr)
		{
			Vector3 v = tr.localScale;
			Transform parent = tr.parent;
			while (parent != null)
			{
				v.Scale(parent.localScale);
				parent = parent.parent;
			}
			return v;
		}

		private static void GetComponentsInternal<TComponent, TResult>(Transform transform, bool includeInactive, Type stopType, List<TResult> result)
			where TComponent : Component, TResult
		{
			int count = transform.childCount;
			for (int i = 0; i < count; ++i)
			{
				Transform child = transform.GetChild(i);
				if (!child.gameObject.activeSelf && !includeInactive)
					continue;

				var stopTarget = child.GetComponent(stopType);
				if (stopTarget != null)
				{
					if (stopTarget is TComponent)
					{
						result.Add((TComponent)stopTarget);
					}
					continue;
				}

				var target = child.GetComponent<TComponent>();
				if (target != null)
				{
					result.Add(target);
				}
				GetComponentsInternal<TComponent, TResult>(child, includeInactive, stopType, result);
			}
		}

		private static JsonData SimpleSerialize(UCodeModelGenerator tr)
		{
			JsonData node = new JsonData();
			node["name"] = tr.mPartName;
			node["position"] = new JsonData();
			node["position"]["x"] = new JsonData(tr.transform.position.x);
			node["position"]["y"] = new JsonData(tr.transform.position.y);
			node["position"]["z"] = new JsonData(tr.transform.position.z);
			node["rotation"] = new JsonData();
			node["rotation"]["x"] = new JsonData(tr.transform.rotation.x);
			node["rotation"]["y"] = new JsonData(tr.transform.rotation.y);
			node["rotation"]["z"] = new JsonData(tr.transform.rotation.z);
			node["rotation"]["w"] = new JsonData(tr.transform.rotation.w);
			node["scale"] = new JsonData();
			var lossyScale = GetGlobalScale(tr.transform);
			node["scale"]["x"] = new JsonData(lossyScale.x);
			node["scale"]["y"] = new JsonData(lossyScale.y);
			node["scale"]["z"] = new JsonData(lossyScale.z);
			return node;
		}

		private static JsonData SimpleDeserialize(Transform tr, JsonData node)
		{
			try
			{
				tr.name = (string)node["name"];
				var position = node["position"];
				var rotation = node["rotation"];
				var scale = node["scale"];
				tr.position = new Vector3((float)(double)position["x"], (float)(double)position["y"], (float)(double)position["z"]);
				tr.rotation = new Quaternion((float)(double)rotation["x"], (float)(double)rotation["y"], (float)(double)rotation["z"], (float)(double)rotation["w"]);
				tr.localScale = new Vector3((float)(double)scale["x"], (float)(double)scale["y"], (float)(double)scale["z"]);
				return node;
			}
			catch (Exception ex)
			{
				DebugUtility.LogError(LoggerTags.Project, ex.Message);
			}
			return node;
		}

		[UnityEditor.MenuItem("GameObject/UCodeModelGenerator/CopySelection", priority = 0)]
		private static void CopySelection()
		{
			if (UnityEditor.Selection.activeTransform != null)
			{
				CopyHierarchy(UnityEditor.Selection.activeTransform);
			}
		}

		public static void CopyHierarchy(Transform transform)
		{
			var hierarchy = new JsonData();
			var components = new List<UCodeModelGenerator>();
			GetComponentsInternal<UCodeModelGenerator, UCodeModelGenerator>(transform, true, typeof(UCodeModelGenerator), components);
			foreach (var r in components)
			{
				hierarchy.Add(SimpleSerialize(r));
			}

			var root = new JsonData();
			root["hierarchy"] = hierarchy;

			GUIUtility.systemCopyBuffer = root.ToJson();
		}

#if UBTROBOT

		[UnityEditor.MenuItem("GameObject/UCodeModelGenerator/PasteRobot", priority = 0)]
		public static void PasteRobot()
		{
			UnityEditor.Selection.objects = new UnityObject[0];
			PasteHierarchy();
			var objects = UnityEditor.Selection.objects;
			CreateRobotTree(objects, false);
		}

		[UnityEditor.MenuItem("GameObject/UCodeModelGenerator/PasteHierarchy", priority = 0)]
		public static void PasteHierarchy()
		{
			var systemCopyBuffer = GUIUtility.systemCopyBuffer;
			if (string.IsNullOrEmpty(systemCopyBuffer))
			{
				return;
			}

			if (!systemCopyBuffer.Contains("hierarchy"))
			{
				DebugUtility.LogError(LoggerTags.Project, "It's not the engine format.");
				return; 
			}

			var root = LitJson.JsonMapper.ToObject(systemCopyBuffer);
			if (root.ContainsKey("hierarchy"))
			{
				var hierarchy = root["hierarchy"];
				if (hierarchy.IsArray)
				{
					int hierarchyCount = hierarchy.Count;
					GameObject robot = new GameObject(Guid.NewGuid().ToString());
					Transform robotRoot = robot.transform;
					for (int i = 0; i < hierarchyCount; ++i)
					{
						GameObject go = new GameObject(i.ToString());
						go.transform.SetParent(robotRoot, true);
						SimpleDeserialize(go.transform, hierarchy[i]);
					}
					UnityEditor.Selection.activeTransform = robotRoot;
				}
			}
		}

		[UnityEditor.MenuItem("GameObject/UCodeModelGenerator/ResetHierarchy", priority = 0)]
		public static void ResetHierarchy()
		{
			var objects = UnityEditor.Selection.objects;
			ResetHierarchy(objects);
		}

		[UnityEditor.MenuItem("GameObject/UCodeModelGenerator/CreateRobotTree", priority = 0)]
		public static void CreateRobotTree()
		{
			var objects = UnityEditor.Selection.objects;
			ResetHierarchy(objects);
			CreateRobotTree(objects, false);
		}

		private static void ResetHierarchy(UnityObject[] objects)
		{
			if (objects != null && objects.Length > 0)
			{
				for (int i = 0; i < objects.Length; ++i)
				{
					var go = objects[i] as GameObject;
					if (go == null)
					{
						continue;
					}
					var activeTransform = go.transform;
					if (activeTransform != null)
					{
						var children = new List<Transform>();
						for (int j = 0; j < activeTransform.childCount; ++j)
						{
							children.Add(activeTransform.GetChild(j));
						}

						for (int j = 0; j < children.Count; ++j)
						{
							var child = children[j];
							var newChild = new GameObject(child.name);
							newChild.transform.SetParent(activeTransform, false);
							newChild.transform.CopyFrom(child);
						}

						for (int j = 0; j < children.Count; ++j)
						{
							DestroyImmediate(children[j].gameObject, false);
						}
					}
				}
			}
		}

		private static void CreateRobotTree(UnityObject[] objects, bool newTree)
		{
			if (objects != null && objects.Length > 0)
			{
				for (int i = 0; i < objects.Length; ++i)
				{
					var go = objects[i] as GameObject;
					if (go == null)
					{
						continue;
					}

					var newGo = newTree ? UnityObject.Instantiate(go) : go;
					newGo.name = go.name;
					var root = newGo.transform;
					if (root)
					{
						var library = UbtrobotSettings.GetOrLoad().partsLibrary;

						var children = new List<Transform>();
						var toRemoveChildren = new List<Transform>();

						var it = root.GetChildren();
						while (it != null && it.MoveNext())
						{
							children.Add(it.Current);
						}

						foreach (Transform tr in children)
						{
							var prefab = library.LoadPrefab(tr.name);
							if (prefab != null)
							{
								prefab.transform.SetParent(root, false);
								prefab.transform.CopyFrom(tr);
							}
							else
							{
								DebugUtility.LogError(LoggerTags.AssetManager, "Can't create part with {0}", tr.name);
							}
						}

						children.ForEach(tr =>
						{
							if (toRemoveChildren.Contains(tr))
							{
								DestroyImmediate(tr.gameObject, false);
							}
						});
						children.RemoveAll(tr => tr == null);

						foreach (var tr in children)
						{
							tr.transform.SetParent(root, false);
						}
					}
				}
			}
		}
#endif
#endif

	}
}
