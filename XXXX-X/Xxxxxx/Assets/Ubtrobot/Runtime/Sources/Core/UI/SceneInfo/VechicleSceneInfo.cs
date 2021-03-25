using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class VechicleSceneInfo : SceneInfo
	{
		public float actionRadius = 20f;
		private Vector3 oldPos;

		private Transform vechicle;
		private Transform vechicleRobot
		{
			get
			{
				if (vechicle == null)
				{
					if (modelHandle.childCount > 0)
					{
						vechicle = modelHandle.GetChild(0);
						oldPos = vechicleRobot.position;
					}
				}
				return vechicle;
			}
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();

			if (modelHandle == null)
			{
				DebugUtility.LogError(LoggerTags.Project, "ModelHandle is null");
				return;
			}
		}

		private void Update()
		{
			if (vechicleRobot == null)
			{
				return;
			}

			if (!Misc.Nearly(vechicleRobot.position, oldPos)
				&& (modelHandle.position - vechicleRobot.position).sqrMagnitude > actionRadius * actionRadius)
			{
				vechicleRobot.position = Vector3.zero;
			}
		}
	}
}
