using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Loki
{
	public abstract class UObjectEditor<TMostDerived> : LokiEditor<TMostDerived> where TMostDerived : UObjectEditor<TMostDerived>
	{

	}

	[CanEditMultipleObjects, CustomEditor(typeof(UObject), true)]
	public sealed class LokiUObjectEditor : UObjectEditor<LokiUObjectEditor>
	{

	}

}
