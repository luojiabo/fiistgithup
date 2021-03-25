using UnityEngine;
using System;
using System.Collections.Generic;

namespace Loki
{
	/// <summary>
	/// The Object supported Component cache, it will cost some memory for against the CPU cost
	/// </summary>
	public abstract class UObjectCache : UObject
	{
		private readonly Dictionary<Type, Component> mComponentCaches = new Dictionary<Type, Component>();

		/// <summary>
		/// The high performance API for GetComponent
		/// </summary>
		/// <typeparam name="TMostDerived">The Most Derived Type</typeparam>
		/// <returns></returns>
		public Component GetComponentCache(Type type)
		{
			DebugUtility.Assert(this != null, "this MonoBehaviour has destroyed.");

			mComponentCaches.TryGetValue(type, out var component);
			if (component == null)
			{
				component = GetComponent(type);
				if (component == null)
				{
					mComponentCaches.Remove(type);
				}
				else
				{
					mComponentCaches[type] = component;
				}
			}
			return component;
		}

		/// <summary>
		/// The high performance API for GetComponent
		/// </summary>
		/// <typeparam name="TMostDerived">The Most Derived Type</typeparam>
		/// <returns></returns>
		public TMostDerived GetComponentCache<TMostDerived>() where TMostDerived : Component
		{
			DebugUtility.Assert(this != null, "this MonoBehaviour has destroyed.");
			var type = typeof(TMostDerived);
			mComponentCaches.TryGetValue(type, out var component);
			if (component == null)
			{
				TMostDerived com = GetComponent<TMostDerived>();
				if (com == null)
				{
					mComponentCaches.Remove(type);
				}
				else
				{
					mComponentCaches[type] = com;
				}
				return com;
			}
			return component as TMostDerived;
		}

	}
}
