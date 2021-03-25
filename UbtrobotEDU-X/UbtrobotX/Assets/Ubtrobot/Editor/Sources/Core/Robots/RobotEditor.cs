using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ubtrobot
{
	public abstract class RobotEditor<T> : Loki.LokiEditor<T> where T : RobotEditor<T>
	{
		protected readonly List<UnityObject> mAllInjectObjects = new List<UnityObject>();

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		private void GenerateAllInjectEditors()
		{
			mAllInjectObjects.Clear();

			Robot robot = target as Robot;
			if (robot)
			{
				robot.ForceUpdate();

				foreach (var p in robot.parts)
				{
					Part part = p as Part;
					if (part)
					{
						foreach (var t in part.commandHandlers)
						{
							if (t is PartComponent)
							{
								var pc = (PartComponent)t;
								mAllInjectObjects.Add(pc);
							}
						}
						//mAllInjectObjects.Add(part);
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

	[CanEditMultipleObjects, CustomEditor(typeof(Robot), true)]
	public sealed class RobotEditor : RobotEditor<RobotEditor>
	{


		protected override void OnDrawInspectorGUI()
		{
			base.OnDrawInspectorGUI();


		}
	}


}
