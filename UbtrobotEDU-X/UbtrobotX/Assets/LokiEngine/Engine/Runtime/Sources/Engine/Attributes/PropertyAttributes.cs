using System;
using UnityEngine;
using System.Collections.Generic;

namespace Loki
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ReadOnlyAttribute : PropertyAttribute
	{

	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AssetPathToObjectAttribute : PropertyAttribute
	{
		public bool readOnlyPath { get; set; } = true;
		public bool readOnlyObject { get; set; } = false;
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class SceneNameAttribute : PropertyAttribute
	{
	}
}
