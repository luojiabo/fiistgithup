using System.Collections;
using System.Collections.Generic;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class SceneViewerCamera : CameraController, IUpdatable
	{
		public bool followTarget = false;
		private Vector3 offset;
		private Transform target;
		private Vector3 robotOldPos;

		private Transform targetRobot
		{
			get
			{
				if (target != null)
				{
					return target;
				}

				RobotManager robotManager = ModuleManager.Get().GetSystemChecked<RobotManager>();
				if (robotManager == null)
				{
					return null;
				}

				var firstRobot = robotManager.firstRobot;
				if (firstRobot != null)
				{
					target = firstRobot.transform;
					if (target != null)
					{
						robotOldPos = target.position;
						offset = transform.position - target.position;
					}
				}
				return target;
			}
		}

		public void OnUpdate(float deltaTime)
		{
			if (targetRobot == null)
			{
				return;
			}

			if (followTarget && !Misc.Nearly(targetRobot.position, robotOldPos))
			{
				transform.position = target.position + offset;
				robotOldPos = target.position;
			}
		}
	}

}
