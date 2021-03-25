using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Loki
{
	public abstract class UComponentEditor<TMostDerived> : LokiEditor<TMostDerived> where TMostDerived : UComponentEditor<TMostDerived>
	{
		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();
		}

		protected override void OnDrawCustomMethods(string category)
		{
			base.OnDrawCustomMethods(category);
		}
	}


	[CanEditMultipleObjects, CustomEditor(typeof(UComponent), true)]
	public sealed class LokiUComponentEditor : UComponentEditor<LokiUComponentEditor>
	{
		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();
		}

		protected override void OnDrawCustomMethods(string category)
		{
			base.OnDrawCustomMethods(category);
		}
	}
}
