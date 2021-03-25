using System;
using System.Collections.Generic;
using System.Linq;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	/// <summary>
	/// 亮度传感器
	/// </summary>
	[DynamicSceneDrawer(sceneTitle = "亮度传感器", tooltip = "测量环境亮度")]
	public class LuminanceSensorComponent : UKitComponent
	{
		protected override bool OnVerify(ICommand command)
		{
			if (command is UKitCommands.LuminanceCommand)
			{
				return true;
			}

			return false;
		}

		public override DriversType driversType => DriversType.UKitLuminanceSensor;

		protected override IProtocol ExecuteUKitCommands(ICommand command)
		{
			ExploreProtocol result = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command);

			do
			{
				if (command is UKitCommands.LuminanceCommand)
				{
					debug = command.debug;

					if (GetEnvironment(out var env))
					{
						//var cmd = (UKitCommands.LuminanceCommand)command;
						result.SetDatas(Mathf.CeilToInt(env.luminance));
						result.code = 0;
						DebugUtility.Log(LoggerTags.Project, "LuminanceCommand Reacted : {0}(lm)", env.luminance);
					}
					break;
				}
			} while (false);

			return result;
		}
	}
}
