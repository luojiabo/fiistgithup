using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Loki
{
	/// <summary>
	/// Always use the UObject instead of the MonoBehaviour
	/// </summary>
	public abstract class UObject : UObjectUtility, IConsoleObject
	{
		public string statID { get { return name; } }

		public static bool hasQuitApplication { get; protected set; } = false;

		public static TMostDerived NewImmortalObject<TMostDerived>(string name, string moduleName) where TMostDerived : UObject
		{
			TMostDerived o = NewObject<TMostDerived>(name);
			if (o != null)
			{
				o.AsImmortal(moduleName);
				return o;
			}
			return null;
		}

		public static UObject NewImmortalObject(Type type, string name, string moduleName)
		{
			UObject o = NewObject(type, name);
			if (o != null)
			{
				UGlobalObject.Get().AddGlobalObject(o, moduleName);
				return o;
			}
			return null;
		}

		/// <summary>
		/// NewObject with type
		/// </summary>
		/// <typeparam name="TMostDerived">the topmost derivered type</typeparam>
		/// <param name="name">the gameObject name</param>
		/// <param name="dontDestroyOnLoad">does the gameObject not destroy on load</param>
		/// <returns></returns>
		public static TMostDerived NewObject<TMostDerived>(string name) where TMostDerived : UObject
		{
			if (hasQuitApplication)
			{
				DebugUtility.AssertFormat(hasQuitApplication, "Please do not create object while Application quit");
				return null;
			}

			var go = new GameObject(name);
			return go.AddComponent<TMostDerived>();
		}

		/// <summary>
		/// NewObject with type
		/// </summary>
		/// <param name="type">the topmost derivered type</param>
		/// <param name="name">the gameObject name</param>
		/// <param name="dontDestroyOnLoad">does the gameObject not destroy on load</param>
		/// <returns></returns>
		public static UObject NewObject(Type type, string name)
		{
			if (hasQuitApplication)
			{
				DebugUtility.AssertFormat(hasQuitApplication, "Please do not create object while Application quit");
				return null;
			}

#if UNITY_EDITOR
			if (!type.IsSubclassOf<UObject>(false))
			{
				return null;
			}
#endif
			var go = new GameObject(name, type);
			return go.GetComponent(type) as UObject;
		}

		public static Object NewObject(Object original)
		{
			if (original == null)
				return null;

			ProfilingUtility.BeginSample("UObject.NewObject_", original.name);
			Object result = Instantiate(original);
			ProfilingUtility.EndSample();

			return result;
		}

		public static Object NewObject(Object original, Vector3 position, Quaternion rotation)
		{
			if (original == null)
				return null;

			ProfilingUtility.BeginSample("UObject.NewObject_", original.name);
			Object result = Instantiate(original, position, rotation);
			ProfilingUtility.EndSample();

			return result;
		}

		public static Object NewObject(Object original, Vector3 position, Quaternion rotation, Transform parent)
		{
			if (original == null)
				return null;

			ProfilingUtility.BeginSample("UObject.NewObject_", original.name);
			Object result = Instantiate(original, position, rotation, parent);
			ProfilingUtility.EndSample();

			return result;
		}

		public static T NewObject<T>(T original) where T : Object
		{
			if (original == null)
				return null;

			ProfilingUtility.BeginSample("UObject.NewObject_", original.name);
			T result = Instantiate<T>(original);
			ProfilingUtility.EndSample();

			return result;
		}

		public static T NewObject<T>(T original, Transform parent) where T : Object
		{
			if (original == null)
				return null;

			ProfilingUtility.BeginSample("UObject.NewObject_", original.name);
			T result = Instantiate<T>(original, parent);
			ProfilingUtility.EndSample();

			return result;
		}

		public static T NewObject<T>(T original, Transform parent, bool worldPositionStays) where T : Object
		{
			if (original == null)
				return null;

			ProfilingUtility.BeginSample("UObject.NewObject_", original.name);
			T result = Instantiate<T>(original, parent, worldPositionStays);
			ProfilingUtility.EndSample();
			return result;
		}

		public static T NewObject<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
		{
			if (original == null)
				return null;

			ProfilingUtility.BeginSample("UObject.NewObject_", original.name);
			T result = Instantiate<T>(original, position, rotation, parent);
			ProfilingUtility.EndSample();
			return result;
		}

		public static T NewObject<T>(T original, Vector3 position, Quaternion rotation) where T : Object
		{
			if (original == null)
				return null;

			ProfilingUtility.BeginSample("UObject.NewObject_", original.name);
			T result = Instantiate<T>(original, position, rotation);
			ProfilingUtility.EndSample();
			return result;
		}

		/// <summary>
		/// Must call from subclass
		/// </summary>
		protected virtual void Awake()
		{
			this.RegisterToConsole();
		}

		/// <summary>
		/// Must call from subclass
		/// </summary>
		protected virtual void OnDestroy()
		{
			this.UnregisterFromConsole();
		}

		public UObject AsImmortal(string moduleName)
		{
			return UGlobalObject.Get().AddGlobalObject(this, moduleName);
		}
	}
}
