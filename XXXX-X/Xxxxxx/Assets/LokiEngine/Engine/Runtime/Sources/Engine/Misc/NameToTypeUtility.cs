using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Loki
{
	public class NameToTypeUtility
	{
		private static readonly Dictionary<string, Type> msTypes = new Dictionary<string, Type>();
		private static readonly List<Type> msAdditionalTypes = new List<Type>()
		{
			typeof(BoxCollider),
			typeof(SphereCollider),
			typeof(MeshCollider),
			typeof(CapsuleCollider),

			typeof(Rigidbody),

			typeof(Camera),
			typeof(Light),
			typeof(AudioListener),
			typeof(Animation),
			typeof(AudioSource),

			typeof(ParticleSystem),

			typeof(MeshRenderer),
			typeof(SkinnedMeshRenderer),
		};

		private static bool mInitialize = false;

		public static void Initialize(bool force = false)
		{
			if (mInitialize)
				return;
			mInitialize = true;

			var types = GlobalReflectionCache.FindTypes<NameToTypeAttribute>(false, msAdditionalTypes);
			foreach (var item in types)
			{
				RegisterType(item);
			}
#if UNITY_EDITOR
			DebugUtility.Log(LoggerTags.Engine, "Register types : {0}", types.Select(type => type.Name));
#endif
		}

		public static bool TryGetValue(string typeName, out Type type)
		{
			Initialize(false);
			return msTypes.TryGetValue(typeName, out type);
		}

		public static void RegisterType<T>()
		{
			RegisterType(typeof(T));
		}

		public static void RegisterType(Type type)
		{
			msTypes[type.Name] = type;
		}
	}
}
