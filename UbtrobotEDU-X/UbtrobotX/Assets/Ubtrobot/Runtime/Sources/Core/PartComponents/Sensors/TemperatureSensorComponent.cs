using System;
using System.Collections.Generic;
using System.Linq;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	/// <summary>
	/// 温湿度传感器
	/// </summary>
	[DynamicSceneDrawer(sceneTitle = "温湿度传感器", tooltip = "测量温度、湿度")]
	public class TemperatureSensorComponent : UKitComponent
	{
		public static float C2F(float C)
		{
			float F = (9.0f / 5.0f) * C + 32.0f;
			return F;
		}

		public static float F2C(float F)
		{
			var C = (5.0f / 9.0f) * (F - 32.0f);
			return C;
		}

		public override DriversType driversType => DriversType.UKitTemperatureAndHumidity;

		protected override bool OnVerify(ICommand command)
		{
			if (command is UKitCommands.TemperatureCommand)
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
				if (command is UKitCommands.TemperatureCommand)
				{
					debug = command.debug;
					if (GetEnvironment(out var env))
					{
						var cmd = (UKitCommands.TemperatureCommand)command;
						if (cmd.mode == 0)
						{
							var temp = Mathf.CeilToInt(env.temperature);
							result.SetDatas(temp);
							DebugUtility.Log(LoggerTags.Project, "TemperatureCommand Reacted : {0}C", temp.ToString());
						}
						else if (cmd.mode == 1)
						{
							var f = Mathf.CeilToInt(C2F(env.temperature));
							result.SetDatas(f);
							DebugUtility.Log(LoggerTags.Project, "TemperatureCommand Reacted : {0}F", f.ToString());
						}
						else if (cmd.mode == 2)
						{
							var f = Mathf.CeilToInt(env.humidity);
							result.SetDatas(f);
							DebugUtility.Log(LoggerTags.Project, "TemperatureCommand Reacted : {0}%", f.ToString());
						}
						result.code = 0;
					}
					break;
				}
			} while (false);

			return result;
		}

	}
}
