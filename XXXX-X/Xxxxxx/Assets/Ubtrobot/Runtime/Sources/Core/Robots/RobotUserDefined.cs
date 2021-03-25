using System;
using System.Collections.Generic;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class RobotUserDefined : Robot
	{
		// public float rotateSpeed = 0.0f;

		//private void Update()
		//{
		//	if (rotateSpeed > 0.0f)
		//		transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
		//}

		[ConsoleMethod(aliasName = "pf.hide")]
		public void Hide(string partName)
		{

		}

		[ConsoleMethod(aliasName = "pf.show")]
		public void Show(string partName)
		{

		}
	}
}
