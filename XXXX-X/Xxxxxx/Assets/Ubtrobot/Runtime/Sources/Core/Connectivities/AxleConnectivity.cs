using System;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	[DynamicSceneDrawer(sceneTitle = "AxleConnectivity")]
	public class AxleConnectivity : Connectivity
	{
		[SerializeField]
		private AxleJointComponent mJoint;

		[Tooltip("self-rotation or axle-rotation")]
		[SerializeField]
		private bool mIsSelfRotation = false;

		public bool isSelfRotation
		{
			get
			{
				return mIsSelfRotation;
			}
			set
			{
				mIsSelfRotation = value;
			}
		}

		public AxleJointComponent joint
		{
			get
			{
				return mJoint;
			}
			set
			{
				mJoint = value;
			}
		}

		public override EPhysicalType connType
		{
			get { return EPhysicalType.Axle; }
		}

		public Vector3 localEulerAngles
		{
			get
			{
				return transform.localEulerAngles;
			}
			set
			{
				transform.localEulerAngles = value;
			}
		}

#if UNITY_EDITOR
		protected string DynamicDrawerName(string title)
		{
			string ctrlName;
			if (controller != null)
			{
				ctrlName = controller.name;
			}
			else
			{
				ctrlName = "None";
			}

			return string.Format("[{2}]:{0}-({1})", title, localEulerAngles.ToString(), ctrlName);
		}
#endif
		public void Rotate(float y)
		{
			// 如果是组自旋转，则轴保持原位不动
			if (!mIsSelfRotation)
			{
				RotateAxis(y);
			}
			else
			{
				RotateAxis(-y);
			}
			//var localEulerAngles = transform.localEulerAngles;
			//localEulerAngles.y += y;
			//transform.localEulerAngles = localEulerAngles;
			if (mJoint != null)
			{
				mJoint.Rotate(y);
			}
		}

	}
}
