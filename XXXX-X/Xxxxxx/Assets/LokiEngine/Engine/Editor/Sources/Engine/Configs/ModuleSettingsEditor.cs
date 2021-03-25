using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace Loki
{
	public abstract class ModuleSettingsEditor<TMostDerived> : UAssetObjectEditor<TMostDerived> where TMostDerived : ModuleSettingsEditor<TMostDerived>
	{
		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			// CreateChildEditorView(LokiEditorSelection.lastSubAssetObject);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}
	}

	[CustomEditor(typeof(ModuleSettings), true)]
	public sealed class ModuleSettingsEditor : ModuleSettingsEditor<ModuleSettingsEditor>
	{
		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();
		}

	}
}
