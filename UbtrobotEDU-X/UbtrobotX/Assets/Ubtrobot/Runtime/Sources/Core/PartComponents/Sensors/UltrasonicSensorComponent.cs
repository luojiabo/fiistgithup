using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Loki;
using UnityEngine;
using UnityEngine.UI;
using LineRenderer = UnityEngine.LineRenderer;

namespace Ubtrobot
{
	/// <summary>
	/// 超声波传感器，测距
	/// </summary>
	[DynamicSceneDrawer(sceneTitle = "超声波传感器", tooltip = "测距")]
	public class UltrasonicSensorComponent : LineTraceSensorComponent
	{
		private void Reset()
		{
			name = "UltrasonicRange";
		}

		public override DriversType driversType => DriversType.UKitUltrasonicSensor;

		protected override bool OnVerify(ICommand command)
		{
			if (command is UKitCommands.UltrasonicCommand)
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
				if (command is UKitCommands.UltrasonicCommand)
				{
					debug = command.debug;
					//var cmd = (UKitCommands.UltrasonicCommand)command;
					if (LineTrace(out var distance))
					{
						result.SetDatas((int)(distance * 100.0f));
						result.code = 0;

						DebugUtility.Log(LoggerTags.Project, "UltrasonicCommand Reacted : {0}m", distance);
					}
					break;
				}

			} while (false);

			return result;
		}
	}
}
