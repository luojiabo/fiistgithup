using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Loki
{
	public abstract class LokiScriptableObjectEditor<TMostDerived> : LokiEditor<TMostDerived> where TMostDerived : LokiScriptableObjectEditor<TMostDerived>
	{
		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();


		}
	}

	[CustomEditor(typeof(ScriptableObject), true)]
	public sealed class LokiScriptableObjectEditor : LokiScriptableObjectEditor<LokiScriptableObjectEditor>
	{
		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();

		}
	}
}
