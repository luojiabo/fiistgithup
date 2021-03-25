using System;
using System.Collections.Generic;
using System.Linq;
using Loki;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ubtrobot
{
	/// <summary>
	/// 声音传感器
	/// </summary>
	[DynamicSceneDrawer(sceneTitle = "声音传感器", tooltip = "返回当前环境的声音大小")]
	public class SoundSensorComponent : UKitComponent
	{
		protected override bool OnVerify(ICommand command)
		{
			if (command is UKitCommands.SoundCommand)
			{
				return true;
			}

			return false;
		}

		protected override IProtocol ExecuteUKitCommands(ICommand command)
		{
			ExploreProtocol result = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command);
			do
			{
				if (command is UKitCommands.SoundCommand)
				{
					debug = command.debug;
					if (GetEnvironment(out var env))
					{
						//var cmd = (UKitCommands.SoundCommand)command;
						result.SetDatas(Mathf.CeilToInt(env.soundVolume));
						result.code = 0;
						DebugUtility.Log(LoggerTags.Project, "SoundCommand Reacted : {0}", env.soundVolume);
					}
					break;
				}
			} while (false);

			return result;
		}
	}
}
