using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Loki;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ubtrobot
{
	public sealed class PartsGroup : Group, IPartsGroup
	{
		private IRobot mRobot = null;

		public override IRobot robot
		{
			get
			{
				if (mRobot == null)
				{
					if (this.transform.parent != null)
					{
						mRobot = this.transform.parent.GetComponentInParent<Robot>();
					}
				}
				return mRobot;
			}
		}

		public IRobot owner
		{
			get
			{
				return robot;
			}
		}

		public string ExportName(int nameID)
		{
			return string.Concat(GetAddressName(), "@", nameID.ToString());
		}

		public string GetAddressName()
		{
			string aName = name;
			if (string.IsNullOrEmpty(aName))
				return string.Empty;

			var idxOfID = aName.LastIndexOf('@');
			if (idxOfID >= 0)
				return aName.Substring(0, idxOfID);
			return aName;
		}

		[InspectorMethod(aliasName = "CheckCommands")]
		public void CheckCommands()
		{
			Robot robot = GetComponent<Robot>();
			if (robot == null)
			{
				robot = transform.GetComponentInParent<Robot>(false);
			}
			if (robot == null)
			{
				DebugUtility.LogError(LoggerTags.Project, "Missing Robot Component.");
				return;
			}
			robot.CheckCommands();
		}

#if UNITY_EDITOR
		[MenuItem("GameObject/Ubtrobot/CreateGroup #&G", priority = 0)]
		private static void CreateGroup()
		{
			Robot robot = null;
			var selectedTransforms = UnityEditor.Selection.transforms;
			var parts = new List<IPart>();
			foreach (var tr in selectedTransforms)
			{
				if (robot == null)
				{
					robot = tr.GetComponentInParent<Robot>(false);
				}
				var part = tr.GetComponent<Part>();
				if (part)
				{
					parts.Add(part);
				}
			}
			if (parts.Count > 0)
			{
				var pg = new GameObject("Group").AddComponent<PartsGroup>();
				pg.AddChildren(parts);
				if (robot != null)
				{
					pg.transform.SetParent(robot.transform, true);
				}
				UnityEditor.Selection.activeTransform = pg.transform;
			}
			//AddChildren(parts);
		}
#endif
	}
}
