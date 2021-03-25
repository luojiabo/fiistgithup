using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Loki
{
	[System.Flags]
	public enum EComponentSearchOption
	{
		None,

		SearchThis = 0x1,
		SearchChildren_Ignore_SearchThisResult = 0x2,

		IncludeTargetComponent_InStop_This = 0x4,
		IncludeTargetComponent_InStop_Children = 0x8,

		Default = SearchThis | SearchChildren_Ignore_SearchThisResult | IncludeTargetComponent_InStop_This,
		DefaultIgnoreComponentInThis = SearchThis | SearchChildren_Ignore_SearchThisResult,
	}

	public enum AxisType
	{
		Y,
		X,
		Z
	}

	public static class EngineExtension
	{
		public static string ToFixedString(this float f, string format = "#0.000000000")
		{
			return f.ToString(format);
		}

		private static bool IsRootJoint(string jointName)
		{
			return string.IsNullOrEmpty(jointName) || jointName == "/" || jointName == ".";
		}

		public static bool IsSubclassOf<T>(this Type type)
		{
			return type.IsSubclassOf<T>(true);
		}

		public static bool IsSubclassOf<T>(this Type type, bool allowAbstract)
		{
			if (!allowAbstract)
			{
				if (type.IsAbstract)
				{
					DebugUtility.LogErrorTrace(LoggerTags.TypeCheck, "The Type({0}) is an abstract class.", type.Name);
					return false;
				}
			}

			Type componentType = typeof(T);
			if (!type.IsSubclassOf(componentType))
			{
				DebugUtility.LogErrorTrace(LoggerTags.TypeCheck, "The Component({0}) is not a subclass of Type({1})", type.Name, componentType.Name);
				return false;
			}
			return true;
		}

		public static bool IsSubclassOf(this Type type, Type parentType, bool allowAbstract)
		{
			if (!allowAbstract)
			{
				if (type.IsAbstract)
				{
					DebugUtility.LogErrorTrace(LoggerTags.TypeCheck, "The Type({0}) is an abstract class.", type.Name);
					return false;
				}
			}

			if (!type.IsSubclassOf(parentType))
			{
				DebugUtility.LogErrorTrace(LoggerTags.TypeCheck, "The Component({0}) is not a subclass of Type({1})", type.Name, parentType.Name);
				return false;
			}
			return true;
		}

		public static T GetOrAllocComponent<T>(this Component component) where T : Component
		{
			var t = component.GetComponent<T>();
			if (t == null)
				return component.gameObject.AddComponent<T>();
			return t;
		}

		public static T GetOrAllocComponent<T>(this GameObject gameObject) where T : Component
		{
			var t = gameObject.GetComponent<T>();
			if (t == null)
				return gameObject.AddComponent<T>();
			return t;
		}

		/// <summary>
		/// Add user component.
		/// </summary>
		/// <typeparam name="T">[NameToType]T.</typeparam>
		/// <param name="componentTypeName"></param>
		/// <returns></returns>
		public static T AddUserComponent<T>(this GameObject gameObject, string componentTypeName) where T : Component
		{
			if (string.IsNullOrEmpty(componentTypeName))
				return null;

			if (NameToTypeUtility.TryGetValue(componentTypeName, out var type))
			{
				return gameObject.AddComponent(type) as T;
			}

			DebugUtility.LogError(LoggerTags.Engine, "Can't add component to {0}, please confirm that whether the class '{1}' has declared the attribute '[NameToType]'", gameObject.name, componentTypeName);
			return null;
		}

		/// <summary>
		/// Add user component.
		/// </summary>
		/// <typeparam name="T">[NameToType]T.</typeparam>
		/// <param name="componentTypeName"></param>
		/// <returns></returns>
		public static T GetUserComponent<T>(this GameObject gameObject, string componentTypeName) where T : Component
		{
			if (string.IsNullOrEmpty(componentTypeName))
				return null;

			if (NameToTypeUtility.TryGetValue(componentTypeName, out var type))
			{
				return gameObject.GetComponent(type) as T;
			}

			DebugUtility.LogError(LoggerTags.Engine, "Can't get the component from {0}, please confirm that whether the class '{1}' has declared the attribute '[NameToType]'", gameObject.name, componentTypeName);
			return null;
		}

		/// <summary>
		/// Add user component.
		/// </summary>
		/// <typeparam name="T">[NameToType]T.</typeparam>
		/// <param name="componentTypeName"></param>
		/// <returns></returns>
		public static T AddUserComponent<T>(this Component transform, string componentTypeName) where T : Component
		{
			return transform.gameObject.AddUserComponent<T>(componentTypeName);
		}

		/// <summary>
		/// Add user component.
		/// </summary>
		/// <typeparam name="T">[NameToType]T.</typeparam>
		/// <param name="componentTypeName"></param>
		/// <returns></returns>
		public static T GetUserComponent<T>(this Component transform, string componentTypeName) where T : Component
		{
			return transform.gameObject.GetUserComponent<T>(componentTypeName);
		}

		public static T GetOrAllocComponent<T>(this Transform component, string componentName) where T : Component
		{
			var t = component.GetComponent<T>();
			if (t == null)
				return component.gameObject.AddUserComponent<T>(componentName);
			return t;
		}

		public static IEnumerator<Transform> GetChildren(this Transform transform)
		{
			var childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				yield return transform.GetChild(i);
			}
		}

		public static IEnumerator<T> GetChildren<T>(this Transform transform) where T : Component
		{
			var childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				var t = transform.GetChild(i).GetComponent<T>();
				if (t != null)
				{
					yield return t;
				}
			}
		}

		public static IEnumerator<Transform> GetChildrenDFS(this Transform transform)
		{
			var childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				var child = transform.GetChild(i);
				yield return child;
				var it = child.GetChildrenDFS();
				while (it != null && it.MoveNext())
				{
					yield return it.Current;
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T">T is typeof(interface)</typeparam>
		/// <param name="type"></param>
		/// <param name="allowAbstract"></param>
		/// <returns></returns>
		public static bool IsImplementOf<T>(this Type type, bool allowAbstract)
		{
			if (!allowAbstract)
			{
				if (type.IsAbstract)
				{
					DebugUtility.LogErrorTrace(LoggerTags.TypeCheck, "The Type({0}) is an abstract class.", type.Name);
					return false;
				}
			}

			Type interfaceType = typeof(T);
			if (!interfaceType.IsInterface)
			{
				DebugUtility.LogErrorTrace(LoggerTags.TypeCheck, "The T({0}) is not Interface", type.Name);
				return false;
			}

			if (!interfaceType.IsAssignableFrom(type))
			{
				DebugUtility.LogErrorTrace(LoggerTags.TypeCheck, "The Component({0}) is not the implement of Interface({1})", type.Name, interfaceType.Name);
				return false;
			}
			return true;
		}

		public static bool IsCompatibleWith(this Type type, Type target, bool allowAbstract = true)
		{
			if (type.IsAbstract && !allowAbstract)
				return false;

			if (target.IsInterface)
				return target.IsAssignableFrom(type);

			if (target.IsClass)
				return type.IsSubclassOf(target);

			if (target.IsEnum && type.IsEnum)
				return target.FullName == type.FullName;

			return false;
		}

		public static Transform FindOrAdd(this Transform transform, string childName)
		{
			if (string.IsNullOrEmpty(childName))
				return transform;

			var moduleMount = transform.Find(childName);
			if (moduleMount == null)
			{
				moduleMount = new GameObject(childName).transform;
				moduleMount.SetParent(transform);
			}
			return moduleMount;
		}

		public static Transform AddChild(this Transform transform, string childName)
		{
			if (string.IsNullOrEmpty(childName))
				return transform;

			var moduleMount = new GameObject(childName).transform;
			moduleMount.SetParent(transform);
			return moduleMount;
		}

		public static void SetPositionAndRotation(this Transform transform, Vector3 position, Quaternion rotation, Space space)
		{
			if (space == Space.World)
			{
				transform.position = position;
				transform.rotation = rotation;
			}
			else
			{
				transform.localPosition = position;
				transform.localRotation = rotation;
			}
		}

		public static void SetPositionAndRotation(this Transform transform, Vector3 position, Quaternion rotation)
		{
			transform.position = position;
			transform.rotation = rotation;
		}

		public static void SetLocalPositionAndRotation(this Transform transform, Vector3 localPosition, Quaternion localRotation)
		{
			transform.localPosition = localPosition;
			transform.localRotation = localRotation;
		}

		public static Transform FindUnique(this Transform transform, string name, StringComparison comparison = StringComparison.Ordinal)
		{
			ProfilingUtility.BeginSample("FindUnique_" + name);
			Transform result = null;
			if (IsRootJoint(name))
			{
				result = transform;
			}
			else
			{
				result = transform.FindUniqueInternal(name, comparison);
			}
			ProfilingUtility.EndSample();

			return result;
		}

		private static Transform FindUniqueInternal(this Transform transform, string name, StringComparison comparison = StringComparison.Ordinal)
		{
			if (string.Equals(transform.name, name, comparison))
			{
				return transform;
			}

			int childCount = transform.childCount;
			for (int i = 0; i < childCount; ++i)
			{
				var result = transform.GetChild(i).FindUniqueInternal(name, comparison);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		public static void TransformSnapToGround(this Transform transform, float offset = 0)
		{
			RaycastHit hit;
			Ray ray = new Ray(transform.position, Vector3.down);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				Vector3 position = hit.point;
				position += Vector3.up * offset;
				transform.position = position;
			}
		}

		public static void RandomiseRotation(this Transform transform, Vector3 minRotation, Vector3 maxRotation)
		{
			Vector3 rotation = transform.eulerAngles;
			rotation.x = Random.Range(minRotation.x, maxRotation.x);
			rotation.y = Random.Range(minRotation.y, maxRotation.y);
			rotation.z = Random.Range(minRotation.z, maxRotation.z);
			transform.eulerAngles = rotation;
		}

		public static int GetDepth(this Transform transform)
		{
			int depth = 0;
			GetDepthInternal(transform, ref depth);
			return depth;
		}

		public static int GetDepthFromRoot(this Transform transform, Transform root)
		{
			int depth = 0;
			while (transform.parent != null && transform.parent != root)
			{
				transform = transform.parent;
				depth++;
			}
			return depth;
		}

		private static void GetDepthInternal(Transform transform, ref int depth)
		{
			int currentMaxDepth = 0;
			for (var idx = 0; idx < transform.childCount; idx++)
			{
				var tr = transform.GetChild(idx);
				int currentDepth = 1;
				GetDepthInternal(tr, ref currentDepth);
				currentMaxDepth = Mathf.Max(currentMaxDepth, currentDepth);
			}
			depth += currentMaxDepth;
		}

		public static void DestroyEditorOnly(this GameObject go)
		{
			go.DestroyTag("EditorOnly");
		}

		public static void DestroyTag(this GameObject go, string tag)
		{
			int childCount = go.transform.childCount;
			for (int i = childCount - 1; i >= 0; i--)
			{
				var child = go.transform.GetChild(i);
				if (child.CompareTag(tag))
				{
					Object.DestroyImmediate(child.gameObject, true);
				}
				else
				{
					DestroyEditorOnly(child.gameObject);
				}
			}
		}

		/// <summary>
		/// Returns true if this MonoBehaviour/GameObject has been destroyed
		/// </summary>
		/// <returns>returns true if the MonoBehaviour has destroyed</returns>
		public static bool HasDestroyed(this UnityEngine.Object o)
		{
			// it will return true if the MonoBehaviour has destroyed
			return o == null;
		}

		/// <summary>
		/// Get the component from path
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="component"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static T GetComponent<T>(this Component component, string path) where T : Component
		{
			if (component == null)
				return null;
			Transform child = component.transform.Find(path);
			if (child == null)
				return null;
			return child.GetComponent<T>();
		}

		/// <summary>
		/// Get the component from path
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="go"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static T GetComponent<T>(this GameObject go, string path) where T : Component
		{
			if (go == null)
				return null;
			Transform child = go.transform.Find(path);
			if (child == null)
				return null;
			return child.GetComponent<T>();
		}

		/// <summary>
		/// Get components in children  (TTargetComponent, TTargetComponentAsResult)
		/// </summary>
		/// <typeparam name="TComponent">The target component type</typeparam>
		/// <param name="transform">start from here</param>
		/// <param name="includeInactive">include inactive gameObject</param>
		/// <param name="stopType">The stop condition component type</param>
		/// <param name="result">the result</param>
		/// <typeparam name="TComponent"></typeparam>
		public static void GetComponentsInChildren<TComponent, TResult>(this Transform transform, bool includeInactive, Type stopType, List<TResult> result)
			where TComponent : Component, TResult
		{
			transform.GetComponentsInChildren<TComponent, TResult>(includeInactive, EComponentSearchOption.Default, stopType, result);
		}

		/// <summary>
		/// Get components in children  (TTargetComponent, TTargetComponentAsResult)
		/// </summary>
		/// <typeparam name="TComponent">The target component type</typeparam>
		/// <param name="transform">start from here</param>
		/// <param name="includeInactive">include inactive gameObject</param>
		/// <param name="stopType">The stop condition component type</param>
		/// <param name="result">the result</param>
		/// <typeparam name="TComponent"></typeparam>
		public static void GetComponentsInChildren<TComponent, TResult>(this Transform transform, bool includeInactive, EComponentSearchOption searchOption, Type stopType, List<TResult> result)
		where TComponent : Component, TResult
		{
			ProfilingUtility.BeginSample("GetComponentsInChildren_" + transform.name);
			bool continueToGet = true;
			if ((searchOption & EComponentSearchOption.SearchThis) != 0)
			{
				continueToGet = GetComponentsInternalSelf<TComponent, TResult>(transform, searchOption, stopType, result, true);
			}
			if (continueToGet)
			{
				GetComponentsInternal<TComponent, TResult>(transform, includeInactive, searchOption, stopType, result);
			}
			ProfilingUtility.EndSample();
		}

		/// <summary>
		/// Get components in children (TTargetComponent, TStopComponent, TTargetComponentAsResult)
		/// </summary>
		/// <typeparam name="TComponent">The target component type</typeparam>
		/// <typeparam name="TStopComponent">The stop condition component type</typeparam>
		/// <param name="transform">start from here</param>
		/// <param name="includeInactive">include inactive gameObject</param>
		/// <param name="result">the result</param>
		public static void GetComponentsInChildren<TComponent, TStopComponent, TResult>(this Transform transform, bool includeInactive, List<TResult> result)
			where TComponent : Component, TResult
			where TStopComponent : Component
		{
			transform.GetComponentsInChildren<TComponent, TStopComponent, TResult>(includeInactive, EComponentSearchOption.Default, result);
		}

		/// <summary>
		/// Get components in children (TTargetComponent, TStopComponent, TTargetComponentAsResult)
		/// </summary>
		/// <typeparam name="TComponent">The target component type</typeparam>
		/// <typeparam name="TStopComponent">The stop condition component type</typeparam>
		/// <param name="transform">start from here</param>
		/// <param name="includeInactive">include inactive gameObject</param>
		/// <param name="result">the result</param>
		public static void GetComponentsInChildren<TComponent, TStopComponent, TResult>(this Transform transform, bool includeInactive, EComponentSearchOption searchOption, List<TResult> result)
			where TComponent : Component, TResult
			where TStopComponent : Component
		{
			ProfilingUtility.BeginSample("GetComponentsInChildren_StopType_" + transform.name);
			bool continueToGet = true;
			if ((searchOption & EComponentSearchOption.SearchThis) != 0)
			{
				continueToGet = GetComponentsInternalSelf<TComponent, TStopComponent, TResult>(transform, searchOption, result, true);
			}
			if (continueToGet)
			{
				GetComponentsInternal<TComponent, TStopComponent, TResult>(transform, includeInactive, searchOption, result);
			}
			ProfilingUtility.EndSample();
		}

		private static bool GetComponentsInternalSelf<TComponent, TResult>(Transform transform, EComponentSearchOption searchOption, Component stopTarget, List<TResult> result, bool isThis = false)
			where TComponent : Component, TResult
		{
			bool findTargets = true;
			bool containsStopTarget = false;
			if (stopTarget != null)
			{
				if (isThis)
				{
					findTargets = ((searchOption & EComponentSearchOption.IncludeTargetComponent_InStop_This) != 0);
				}
				else
				{
					findTargets = ((searchOption & EComponentSearchOption.IncludeTargetComponent_InStop_Children) != 0);
				}
				containsStopTarget = true;
			}

			if (findTargets)
			{
				var targets = transform.GetComponents<TComponent>();
				if (targets != null)
				{
					result.AddRange(targets);
				}
			}

			if (isThis && ((searchOption & EComponentSearchOption.SearchChildren_Ignore_SearchThisResult) != 0))
			{
				return true;
			}

			return !containsStopTarget;
		}

		private static bool GetComponentsInternalSelf<TComponent, TResult>(Transform transform, EComponentSearchOption searchOption, Type stopType, List<TResult> result, bool isThis = false)
		where TComponent : Component, TResult
		{
			Component stopTarget = transform.GetComponent(stopType);
			return GetComponentsInternalSelf<TComponent, TResult>(transform, searchOption, stopTarget, result, isThis);
		}

		private static void GetComponentsInternal<TComponent, TResult>(Transform transform, bool includeInactive, EComponentSearchOption searchOption, Type stopType, List<TResult> result)
		where TComponent : Component, TResult
		{
			int count = transform.childCount;

			for (int i = 0; i < count; ++i)
			{
				Transform child = transform.GetChild(i);
				if (!child.gameObject.activeSelf && !includeInactive)
					continue;

				if (GetComponentsInternalSelf<TComponent, TResult>(child, searchOption, stopType, result))
				{
					GetComponentsInternal<TComponent, TResult>(child, includeInactive, searchOption, stopType, result);
				}
			}
		}

		private static bool GetComponentsInternalSelf<TComponent, TStopComponent, TResult>(Transform transform, EComponentSearchOption searchOption, List<TResult> result, bool isThis = false)
			where TComponent : Component, TResult
			where TStopComponent : Component
		{
			Component stopTarget = transform.GetComponent<TStopComponent>();
			return GetComponentsInternalSelf<TComponent, TResult>(transform, searchOption, stopTarget, result, isThis);
		}

		private static void GetComponentsInternal<TComponent, TStopComponent, TResult>(Transform transform, bool includeInactive, EComponentSearchOption searchOption, List<TResult> result)
			where TComponent : Component, TResult
			where TStopComponent : Component
		{
			int count = transform.childCount;
			for (int i = 0; i < count; ++i)
			{
				Transform child = transform.GetChild(i);
				if (!child.gameObject.activeSelf && !includeInactive)
					continue;

				if (GetComponentsInternalSelf<TComponent, TStopComponent, TResult>(child, searchOption, result))
				{
					GetComponentsInternal<TComponent, TStopComponent, TResult>(child, includeInactive, searchOption, result);
				}
			}
		}

		public static Bounds CalcRenerersBounds(this Transform transform)
		{
			Bounds bounds;
			transform.GetBoundWithChildren(out bounds);
			return bounds;
		}

		public static bool GetBoundWithChildren(this Transform transform, out Bounds outBound)
		{
			outBound = new Bounds();
			bool encapsulate = false;
			return transform.GetBoundWithChildren(ref outBound, ref encapsulate);
		}

		public static bool GetBoundWithChildren(this Transform transform, ref Bounds pBound, ref bool encapsulate)
		{
			var didOne = false;

			var renderer = transform.GetComponent<Renderer>();
			if (renderer != null)
			{
				var bound = renderer.bounds;
				if (encapsulate)
				{
					pBound.Encapsulate(bound.min);
					pBound.Encapsulate(bound.max);
				}
				else
				{
					pBound.min = bound.min;
					pBound.max = bound.max;
					encapsulate = true;
				}

				didOne = true;
			}

			// union with bound(s) of any/all children
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				var child = transform.GetChild(i);
				if (GetBoundWithChildren(child, ref pBound, ref encapsulate))
				{
					didOne = true;
				}
			}

			return didOne;
		}

		public static Vector3 GetGlobalScale(this Transform tr)
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

		public static void SetGlobalScale(this Transform tr, Vector3 globalScale)
		{
			tr.localScale = Vector3.one;
			tr.localScale = new Vector3(globalScale.x / tr.lossyScale.x, globalScale.y / tr.lossyScale.y, globalScale.z / tr.lossyScale.z);
		}

		public static void CopyFrom(this Transform tr, Transform source, Space space = Space.Self)
		{
			if (space == Space.Self)
			{
				tr.localPosition = source.localPosition;
				tr.localRotation = source.localRotation;
				tr.localScale = source.localScale;
			}
			else
			{
				tr.position = source.position;
				tr.rotation = source.rotation;
				tr.SetGlobalScale(source.GetGlobalScale());
			}
		}

		public static void Reset(this Transform tr, Space space = Space.Self)
		{
			if (space == Space.Self)
			{
				tr.localScale = Vector3.one;
				tr.localRotation = Quaternion.identity;
				tr.localPosition = Vector3.zero;
			}
			else
			{
				tr.SetGlobalScale(Vector3.one);
				tr.rotation = Quaternion.identity;
				tr.position = Vector3.zero;
			}
		}

		public static void CopyFrom(this Transform tr, Transform source, bool ignoreScale, Space space = Space.Self)
		{
			if (space == Space.Self)
			{
				tr.localPosition = source.localPosition;
				tr.localRotation = source.localRotation;
				if (!ignoreScale)
					tr.localScale = source.localScale;
			}
			else
			{
				tr.position = source.position;
				tr.rotation = source.rotation;
				if (!ignoreScale)
					tr.SetGlobalScale(source.GetGlobalScale());
			}
		}

		public static T GetComponentInParent<T>(this Component component, bool addToThisIfNotExist) where T : Component
		{
			T t = component.GetComponentInParent<T>();
			if (t == null && addToThisIfNotExist)
			{
				t = component.GetOrAllocComponent<T>();
			}
			return t;
		}

		public static T GetComponentInParent<T>(this Component component, bool addToThisIfNotExist, bool includeThisComponent) where T : Component
		{
			T t = null;
			if (includeThisComponent)
			{
				t = component.GetComponent<T>();
			}
			if (t == null)
			{
				t = component.GetComponentInParent<T>();
			}
			if (t == null && addToThisIfNotExist)
			{
				t = component.GetOrAllocComponent<T>();
			}
			return t;
		}

		public static void ExeIgnoreChildren(this Transform transform, Action<Transform> behaviour)
		{
			List<Transform> children = new List<Transform>();
			for (int i = 0; i < transform.childCount; ++i)
			{
				children.Add(transform.GetChild(i));
			}
			transform.DetachChildren();
			try
			{
				if (behaviour != null)
				{
					behaviour(transform);
				}
			}
			catch (Exception ex)
			{
				DebugUtility.LogException(ex);
			}
			foreach (var tr in children)
			{
				tr.transform.SetParent(transform, true);
			}
		}

		public static RectTransform GetRectTransform(this RectTransform transform)
		{
			return transform;
		}

		public static RectTransform GetRectTransform(this Transform transform)
		{
			return transform as RectTransform;
		}

		public static RectTransform GetRectTransform(this Component component)
		{
			return component.transform as RectTransform;
		}

		public static RectTransform GetRectTransform(this GameObject go)
		{
			return go.transform as RectTransform;
		}

		public static string GetComponentInfo(this Component component)
		{
			if (component != null)
			{
				return string.Format("[{0} (Type:{3}, ComponentID:{1}-GameObjectID:{2})]", component.name, component.GetInstanceID(), component.gameObject.GetInstanceID(), component.GetType().Name);
			}
			else
			{
				return "[Empty Component]";
			}
		}

		public static bool SetActive(this Transform transform, string path, bool active)
		{
			var child = transform.Find(path);
			if (child == null)
				return false;

			child.gameObject.SetActive(active);
			return true;
		}

		public static IEnumerator AsEnumerator(this Task task, Action<bool> onCompleted)
		{
			while (!task.IsCompleted)
			{
				yield return null;
			}

			Misc.SafeInvoke(onCompleted, !task.IsFaulted);
		}

		public static Coroutine AsCoroutine(this Task task, MonoBehaviour runner, Action<bool> onCompleted)
		{
			if (runner != null)
			{
				return runner.StartCoroutine(task.AsEnumerator(onCompleted));
			}
			return null;
		}

		public static T FindHierarchyPath<T>(this Transform transform, string hierarchyPath) where T : Component
		{
			if (transform == null)
				return null;
			var tr = transform.FindHierarchyPath(hierarchyPath);
			if (tr != null)
				return tr.GetComponent<T>();
			return null;
		}

		public static Transform FindHierarchyPath(this Transform transform, string hierarchyPath)
		{
			if (transform == null)
				return null;

			if (string.IsNullOrEmpty(hierarchyPath))
				return transform;

			if (hierarchyPath[0] == '/')
			{
				return Misc.Find(hierarchyPath);
			}
			else
			{
				return transform.Find(hierarchyPath);
			}
		}

		public static Transform FindHierarchyPath(this Component componet, string hierarchyPath)
		{
			if (componet == null)
				return null;
			return componet.transform.FindHierarchyPath(hierarchyPath);
		}

		public static T FindHierarchyPath<T>(this Component componet, string hierarchyPath) where T : Component
		{
			if (componet == null)
				return null;
			var tr = componet.transform.FindHierarchyPath(hierarchyPath);
			if (tr != null)
				return tr.GetComponent<T>();
			return null;
		}

		public static List<string> GetHierarchyPath(this GameObject[] objs, List<string> resultCache = null)
		{
			if (resultCache == null)
			{
				resultCache = new List<string>(objs.Length);
			}
			foreach (var item in objs)
			{
				var r = item.GetHierarchyPath();
				if (string.IsNullOrEmpty(r))
					continue;
				resultCache.Add(r);
			}
			return resultCache;
		}

		public static List<string> GetHierarchyPath(this Transform[] transforms, List<string> resultCache = null)
		{
			if (resultCache == null)
			{
				resultCache = new List<string>(transforms.Length);
			}
			foreach (var item in transforms)
			{
				var r = item.GetHierarchyPath();
				if (string.IsNullOrEmpty(r))
					continue;
				resultCache.Add(r);
			}
			return resultCache;
		}

		public static List<string> GetHierarchyPath(this GameObject[] objs, Type stopConditon, bool includeStop = false, List<string> resultCache = null)
		{
			if (resultCache == null)
			{
				resultCache = new List<string>(objs.Length);
			}
			foreach (var item in objs)
			{
				var r = item.GetHierarchyPath(stopConditon, includeStop);
				if (string.IsNullOrEmpty(r))
					continue;
				resultCache.Add(r);
			}
			return resultCache;
		}

		public static List<string> GetHierarchyPath<T>(this T[] components, Type stopConditon, bool includeStop = false, List<string> resultCache = null) where T : Component
		{
			if (resultCache == null)
			{
				resultCache = new List<string>(components.Length);
			}
			foreach (var item in components)
			{
				var r = item.transform.GetHierarchyPath(stopConditon, includeStop);
				if (string.IsNullOrEmpty(r))
					continue;
				resultCache.Add(r);
			}
			return resultCache;
		}

		public static Transform FindRecursion(this Component component, string name, bool includeSelf = false)
		{
			return component.transform.FindRecursion<Transform>(name, includeSelf);
		}

		public static T FindRecursion<T>(this Component component, string name, bool includeSelf = false)
			where T : Component
		{
			return component.transform.FindRecursion<T>(name, includeSelf);
		}

		public static T FindRecursion<T>(this Transform transform, string name, bool includeSelf = false) where T : Component
		{
			if (transform == null) return null;

			ProfilingUtility.BeginSample("FindComponentInRecursion_", transform.name);
			var target = FindRecursionInternal<T>(transform, name, includeSelf);
			ProfilingUtility.EndSample();
			return target;
		}

		private static T FindRecursionInternal<T>(Transform transform, string name, bool includeSelf) where T : Component
		{
			T target = null;

			if (includeSelf && transform.name == name)
			{
				target = transform.GetComponent<T>();
			}

			if (target == null)
			{
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					var child = transform.GetChild(i);
					target = child.FindRecursion<T>(name, true);
					if (target != null)
					{
						break;
					}
				}
			}

			return target;
		}

		public static string GetHierarchyPath(this GameObject obj)
		{
			return obj.transform.GetHierarchyPath();
		}

		public static string GetHierarchyPath(this Transform transform)
		{
			string path = transform.name;
			while (transform.parent != null)
			{
				transform = transform.parent;
				path = string.Concat(transform.name, "/", path);
			}
			return string.Concat("/", path);
		}

		public static string GetHierarchyPath(this GameObject obj, Type stopConditon, bool includeStop = false)
		{
			if (stopConditon != null)
				return obj.transform.GetHierarchyPath(stopConditon, includeStop);
			return obj.transform.GetHierarchyPath();
		}

		public static string GetHierarchyPath(this Transform transform, Type stopConditon, bool includeStop = false)
		{
			if (stopConditon == null)
			{
				return transform.GetHierarchyPath();
			}

			string path = transform.name;
			while (transform.parent != null)
			{
				if (transform.parent.GetComponent(stopConditon))
				{
					if (includeStop)
					{
						transform = transform.parent;
						path = string.Concat(transform.name, "/", path);
					}
					break;
				}

				transform = transform.parent;
				path = string.Concat(transform.name, "/", path);
			}
			if (transform.parent == null)
				return string.Concat("/", path);
			return path;
		}


	}
}
