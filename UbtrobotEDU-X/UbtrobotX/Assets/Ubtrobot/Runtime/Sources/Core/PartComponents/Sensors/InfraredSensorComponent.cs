using System;
using System.Collections.Generic;
using System.Linq;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	/// <summary>
	/// 红外线传感器
	/// </summary>
	[DynamicSceneDrawer(sceneTitle = "红外传感器", tooltip = "测距")]
	public class InfraredSensorComponent : LineTraceSensorComponent
	{
		protected override bool OnVerify(ICommand command)
		{
			if (command is UKitCommands.InfraredCommand)
			{
				return true;
			}

			return false;
		}

		public override DriversType driversType => DriversType.UKitInfraredSensor;

		protected override IProtocol ExecuteUKitCommands(ICommand command)
		{
			ExploreProtocol result = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command);
			do
			{
				if (command is UKitCommands.InfraredCommand)
				{
					debug = command.debug;
					if (LineTrace(out var distance))
					{
						result.SetDatas((int)(distance * 100.0f));
						result.code = 0;

						DebugUtility.Log(LoggerTags.Project, "InfraredCommand Reacted : {0}m", distance);
					}
					break;
				}

			} while (false);

			return result;
		}

		private void Reset()
		{
			name = "Infrared";
		}
	}
}
