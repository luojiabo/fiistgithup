using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityObject = UnityEngine.Object;
using UnityEngine.AI;

namespace Loki
{
	public abstract class UAssetObjectEditor<TMostDerived> : LokiScriptableObjectEditor<TMostDerived> where TMostDerived : UAssetObjectEditor<TMostDerived>
	{
		protected UnityObject mParentTarget;
		protected Editor mParentView;

		protected void DestroyParentView()
		{
			if (mParentView != null)
			{
				DestroyImmediate(mParentView);
				mParentView = null;
			}
			mParentTarget = null;
		}

		protected void CreateParentEditor<T>() where T : UAssetObject
		{
			if (mParentView != null)
				return;

			UAssetObject o = target as UAssetObject;
			if (o == null || !o.isSubAsset)
			{
				return;
			}

			var parent = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GetAssetPath(o));
			if (parent != null)
			{
				mParentTarget = parent;
				mParentView = Editor.CreateEditor(mParentTarget);
			}
		}

		protected void DrawParentView()
		{
			if (mParentView != null)
			{
				DrawLine();
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.ObjectField(mParentTarget, mParentTarget.GetType(), false);
				EditorGUI.EndDisabledGroup();
				mParentView.OnInspectorGUI();
				DrawLine();
			}
		}

		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			DestroyParentView();
			LokiEditorSelection.lastSelectedObject = target;
			base.OnDisable();
		}
	}

	[CustomEditor(typeof(UAssetObject), true)]
	public sealed class UAssetObjectEditor : UAssetObjectEditor<UAssetObjectEditor>
	{
		protected override void OnDrawInspectorGUI()
		{
			DrawParentView();
			base.OnDrawInspectorGUI();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			CreateParentEditor<ModuleSettings>();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}
	}
}
