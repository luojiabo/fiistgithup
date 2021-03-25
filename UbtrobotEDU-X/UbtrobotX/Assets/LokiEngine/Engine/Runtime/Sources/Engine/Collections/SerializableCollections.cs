using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Loki
{
	[Serializable]
	[Collection(valueType = typeof(string))]
	public class SerializableNestListString : List<string> { }

#if UNITY_EDITOR
	[Serializable]
	[Collection(keyType = typeof(BuildTargetGroup), valueType = typeof(SerializableNestListString))]
	public class SerializableDictionaryBLS : SerializableDictionary<BuildTargetGroup, SerializableNestListString> { }
#endif
}
