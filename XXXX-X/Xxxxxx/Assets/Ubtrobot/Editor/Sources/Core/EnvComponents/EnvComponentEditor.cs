using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ubtrobot
{
	public abstract class EnvComponentEditor<TMostDerived> : Loki.UComponentEditor<TMostDerived> where TMostDerived : EnvComponentEditor<TMostDerived>
	{
		protected readonly List<UnityObject> mAllInjectObjects = new List<UnityObject>();

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		private void GenerateAllInjectEditors()
		{
			mAllInjectObjects.Clear();
		}

		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();

			GenerateAllInjectEditors();
			DeclareInjectEditors(mAllInjectObjects);
		}

		protected override void OnEditorUpdate()
		{
			base.OnEditorUpdate();
		}

		protected virtual void OnSceneGUI()
		{
			var component = target as Component;
			OnSceneGUIDrawTitle(component);
		}
	}

	[CanEditMultipleObjects, CustomEditor(typeof(Environment), true)]
	public sealed class EnvComponentEditor : EnvComponentEditor<EnvComponentEditor>
	{
		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();
		}
	}
}
