using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Loki
{
	[CustomPropertyDrawer(typeof(SerializableDictionary<int, string>), true)]
	public sealed class DictionaryIntStringPropertyDrawer : DictionaryPropertyDrawer<int, string>
	{
	}

	[CustomPropertyDrawer(typeof(SerializableDictionary<string, int>), true)]
	public sealed class DictionaryStringIntPropertyDrawer : DictionaryPropertyDrawer<string, int>
	{
	}

	[CustomPropertyDrawer(typeof(SerializableDictionary<BuildTargetGroup, SerializableNestListString>), true)]
	public sealed class DictionaryBLSPropertyDrawer : DictionaryPropertyDrawer<BuildTargetGroup, SerializableNestListString>
	{
	}

}
