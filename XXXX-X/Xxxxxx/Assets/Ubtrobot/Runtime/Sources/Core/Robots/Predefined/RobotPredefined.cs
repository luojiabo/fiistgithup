using System;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	public abstract class RobotPredefined : Robot
	{
		[SerializeField]
		private DefaultFollowCameraController mFollowCamera;

		public override void OnTransformUpdated()
		{
			base.OnTransformUpdated();
			if (mFollowCamera != null)
			{
				mFollowCamera.UpdateFollowTarget();
			}
		}

		public override void OnSelected()
		{
			base.OnSelected();
			if (mFollowCamera != null)
			{
				CameraSystem system = ModuleManager.Get().GetSystemChecked<CameraSystem>();
				if (system != null)
				{
					system.ActiveController(mFollowCamera);
					mFollowCamera.UpdateFollowTarget();
				}
			}
		}

		public override void OnUnselected()
		{
			base.OnUnselected();
			if (mFollowCamera != null)
			{
				CameraSystem system = ModuleManager.Get().GetSystemChecked<CameraSystem>();
				if (system != null)
				{
					system.ActiveControllerByTag(TagUtility.MainCamera);
					if (system.activeController != null)
					{
						system.activeController.transform.CopyFrom(mFollowCamera.transform, Space.World);
					}
				}
			}
		}
	}
}
