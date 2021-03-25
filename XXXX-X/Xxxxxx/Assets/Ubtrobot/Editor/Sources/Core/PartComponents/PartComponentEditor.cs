using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ubtrobot
{
	public abstract class PartComponentEditor<TMostDerived> : Loki.UComponentEditor<TMostDerived> where TMostDerived : PartComponentEditor<TMostDerived>
	{
		protected readonly List<UnityObject> mAllInjectObjects = new List<UnityObject>();

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		private void GenerateAllInjectEditors()
		{
			mAllInjectObjects.Clear();
			//PartComponent partComponent = target as PartComponent;
			//if (partComponent)
			//{
			//	partComponent.ForceUpdate();

			//	foreach (var t in partComponent.connectivities)
			//	{
			//		if (t is Connectivity)
			//		{
			//			var conn = (Connectivity)t;
			//			mAllInjectObjects.Add(conn);
			//		}
			//	}
			//}
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


	[CanEditMultipleObjects, CustomEditor(typeof(PartComponent), true)]
	public sealed class PartComponentEditor : PartComponentEditor<PartComponentEditor>
	{
		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();
		}
	}

}
