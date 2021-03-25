using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class RobotProfiler : UComponent
	{
		public Rotation[] robots;

		private void Start()
		{
			robots = GameObject.FindObjectsOfType<Rotation>();

			SetActiveCount(1);
		}

		[ConsoleMethod(aliasName = "l.shadow")]
		private void SetShadowType(int type)
		{
			Light light = UnityEngine.Object.FindObjectOfType<Light>();
			if (light)
			{
				if (type == 0)
					light.shadows = LightShadows.None;
				if (type == 1)
					light.shadows = LightShadows.Hard;
				if (type == 2)
					light.shadows = LightShadows.Soft;
			}
		}

		[ConsoleMethod(aliasName = "pf.count")]
		private void SetActiveCount(int count)
		{
			if (robots == null)
				return;
			for (int i = 0; i < count; i++)
			{
				var go = robots[i];
				if (go)
					go.gameObject.SetActive(true);
			}
			for (int i = count; i < robots.Length; i++)
			{
				var go = robots[i];
				if (go)
					go.gameObject.SetActive(false);
			}
		}
	}
}
