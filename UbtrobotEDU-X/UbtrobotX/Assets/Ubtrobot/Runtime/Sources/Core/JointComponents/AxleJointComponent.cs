using System;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	public sealed class AxleJointComponent : JointComponent
	{
		public void Rotate(float y)
		{
			if (mControlTarget == null)
				return;
			mControlTarget.Rotate(y);

			//var localEulerAngles = mControlTarget.localEulerAngles;
			//localEulerAngles.y += y;
			//mControlTarget.localEulerAngles = localEulerAngles;
		}
	}
}
