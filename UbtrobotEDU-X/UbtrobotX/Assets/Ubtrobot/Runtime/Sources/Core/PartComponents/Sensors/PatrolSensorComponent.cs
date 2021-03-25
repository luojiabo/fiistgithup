using System;
using System.Collections.Generic;
using System.Linq;
using Loki;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ubtrobot
{
	/// <summary>
	/// 巡线传感器
	/// </summary>
	[DynamicSceneDrawer(sceneTitle = "巡线传感器", tooltip = "识别灰度返回深浅")]
	public class PatrolSensorComponent : LineTraceSensorComponent
	{
		private static readonly HashSet<ECommand> msSupportedCommands = new HashSet<ECommand>
		{
			ECommand.PatrolSensorCommand,
		};
		public override DeviceType deviceID => DeviceType.PatrolSensor;

		public override ICommandResponseAsync Execute(ICommand command)
		{
			IProtocol result = null;
			switch (command.commandID)
			{
				case ECommand.PatrolSensorCommand:
					{
						result = ExecutePatrolCommands(command);
						break;
					}
			}

			if (result == null)
			{
				DebugUtility.LogError(LoggerTags.Project, "Failure to execute command : {0}", command);
			}
			else
			{
				DebugUtility.Log(LoggerTags.Project, "Success to execute command : {0}", command);
			}
			var cra = new CommandResponseAsync(result);
			cra.host = command.host;
			cra.context = command.context;
			return cra;
		}

		public override bool Verify(ICommand command)
		{
			if (command.id != id && command.id != 0)
				return false;

			if (!msSupportedCommands.Contains(command.commandID))
				return false;

			return OnVerify(command);
		}

		protected virtual IProtocol ExecutePatrolCommands(ICommand command)
		{
			ExploreProtocol result = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command);
			do
			{
				if (command is PatrolSensorCommands.PatrolSensorBasicCommand)
				{
					debug = command.debug;
					var cmd = (PatrolSensorCommands.PatrolSensorBasicCommand)command;
					if (LineTrace(out var distance, out Color rgb))
					{
						float gray = Misc.ToGray(rgb);
						result.SetDatas(gray >= 0.5f && cmd.isGray);
						result.code = 0;

						DebugUtility.Log(LoggerTags.Project, "PatrolSensorBasicCommand Reacted : {0}m", distance);
					}
					break;
				}

			} while (false);
			return result;
		}

		protected override bool OnVerify(ICommand command)
		{
			if (command is PatrolSensorCommands.PatrolSensorCommand)
			{
				return true;
			}

			return false;
		}

	}
}
