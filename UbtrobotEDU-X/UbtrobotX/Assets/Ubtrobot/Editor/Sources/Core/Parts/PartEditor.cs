using System;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ubtrobot
{
	public abstract class PartEditor<TMostDerived> : Loki.UComponentEditor<TMostDerived> where TMostDerived : PartEditor<TMostDerived>
	{
		protected readonly List<UnityObject> mAllInjectObjects = new List<UnityObject>();

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		private void GenerateAllInjectEditors()
		{
			mAllInjectObjects.Clear();
			Part part = target as Part;
			if (part)
			{
				part.ForceUpdate();

				foreach (var t in part.commandHandlers)
				{
					if (t is PartComponent)
					{
						var pc = (PartComponent)t;
						mAllInjectObjects.Add(pc);
					}
				}
			}
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
	}

	[CanEditMultipleObjects, CustomEditor(typeof(Part), true)]
	public sealed class PartEditor : PartEditor<PartEditor>
	{
		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();
			Part part = (Part)target;
			if (part.isKinematicReady)
			{
				var robot = part.robot;
				if (robot == null)
					return;

				if (robot.physicsSystem.ExistMotionCollider(part))
				{
					var it = robot.physicsSystem.GetOverlaps(part);
					while (it != null && it.MoveNext())
					{
						EditorGUILayout.ObjectField((Collider)it.Current, typeof(Collider), false);
					}
				}
				return;
			}
		}
	}
}
