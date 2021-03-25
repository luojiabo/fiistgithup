using System;
using System.Collections.Generic;
using Loki;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Ubtrobot
{
	/// <summary>
	/// 控制Robot信息显示的功能
	/// </summary>
	public class RobotHUDWindow : UComponent
	{
		[AutoSerializeField]
		public ScrollRect scrollView;

		[AutoSerializeField]
		public Transform partInfo;

		[AutoSerializeField]
		public Transform content;

		private IRobot activeRobot;

		private readonly Dictionary<string, List<Transform>> mParts = new Dictionary<string, List<Transform>>();


		private void Update()
		{
			if (!RobotManager.Get())
				return;
			var firstRobot = RobotManager.GetOrAlloc().firstRobot;

			if (activeRobot != firstRobot)
			{
				activeRobot = firstRobot;
				RefreshUI();
			}
		}

		private void RefreshUI()
		{
			if (activeRobot == null)
			{
				int childCount = content.childCount;
				while (--childCount >= 0)
				{
					var child = content.GetChild(childCount);
					if (child != partInfo)
					{
						DestroyImmediate(child.gameObject, false);
					}
					else
					{
						 
					}
				}
			}
			else
			{
				mParts.Clear();

			}
		}
	}
}
