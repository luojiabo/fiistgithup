using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	[CreateAssetMenu(menuName = "Loki/Configs/Project/Ubtrobot Gizmos Settings", fileName = "GizmosSettings", order = -999)]
	public class GizmosSettings : UAssetObject
	{
		public Color servoColor = Color.green;
		public bool drawServo = true;

		public Color motorColor = Color.green;
		public bool drawMotor = true;

		public Color robotAABBColor = Color.green;
		public bool drawRobotAABB = true;

		public Color defaultColor { get { return Color.white; } }

	}
}
