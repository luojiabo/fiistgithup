using System;
using System.Collections.Generic;
using System.Linq;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	/// <summary>
	/// 颜色识别器
	/// </summary>
	[DynamicSceneDrawer(sceneTitle = "颜色传感器", tooltip = "返回RGB")]
	public class ColorRecognitionSensorComponent : LineTraceSensorComponent
	{
		private void Reset()
		{
			name = "ColorRecognition";
		}

		public override DriversType driversType => DriversType.UKitColorRecognitionSensor;

		protected override bool OnVerify(ICommand command)
		{
			if (command is UKitCommands.ColorRecognitionCommand)
			{
				return true;
			}

			if (command is UKitCommands.ColorRecognitionRGBCommand)
			{
				return true;
			}

			return false;
		}

		protected override IProtocol ExecuteUKitCommands(ICommand command)
		{
			ExploreProtocol result = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command); do
			{
				if (command is UKitCommands.ColorRecognitionCommand)
				{
					debug = command.debug;
					var cmd = (UKitCommands.ColorRecognitionCommand)command;
					if (LineTrace(out var distance, out Color rgb))
					{
						result.SetDatas(Misc.Nearly(cmd.color, rgb));
						result.code = 0;

						DebugUtility.Log(LoggerTags.Project, "ColorRecognitionCommand Reacted : {0}m", distance);
					}
					break;
				}
				if (command is UKitCommands.ColorRecognitionRGBCommand)
				{
					debug = command.debug;
					//var cmd = (UKitCommands.UltrasonicCommand)command;
					if (LineTrace(out var distance, out Color rgb))
					{
						result.SetDatas((int)(rgb.r * 255), (int)(rgb.g * 255), (int)(rgb.b * 255));
						result.code = 0;

						DebugUtility.Log(LoggerTags.Project, "ColorRecognitionRGBCommand Reacted : {0}m", distance);
					}
					break;
				}

			} while (false);

			return result;
		}
	}
}
