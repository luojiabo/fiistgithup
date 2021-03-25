using System.Collections.Generic;
using Loki;
using LitJson;

namespace Ubtrobot
{
	[DynamicSceneDrawer(sceneTitle = "主控", tooltip = "主控检测")]
	public sealed partial class MainControlPartComponent : AdvancedPartComponent
	{
		private static readonly HashSet<ECommand> msSupportedCommands = new HashSet<ECommand>
		{
			ECommand.MiscCommand,
		};

		public sealed override DeviceType deviceID => DeviceType.MainCtrl;

		public override bool Verify(ICommand command)
		{
			if (!msSupportedCommands.Contains(command.commandID))
				return false;

			return true;
		}

		public override ICommandResponseAsync Execute(ICommand command)
		{
			IProtocol result = null;
			switch (command.commandID)
			{
				case ECommand.MiscCommand:
					{
						result = ExecuteMainControlCommands(command);
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

		public IProtocol ExecuteMainControlCommands(ICommand command)
		{
			ExploreProtocol result = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command);
			do
			{
				if (command is MiscCommands.SetIDCommand)
				{
					var cmd = (MiscCommands.SetIDCommand)command;
					result.code = ChangeID(cmd.targetDevice, cmd.oldID, cmd.newID);
					break;
				}

				if (command is MiscCommands.QueryDeviceCommand)
				{
					var cmd = (MiscCommands.QueryDeviceCommand)command;
					result.code = QueryDevice(result);
					break;
				}

				if (command is MiscCommands.QueryDeviceUpgradableCommand)
				{
					var cmd = (MiscCommands.QueryDeviceUpgradableCommand)command;
					break;
				}

				if (command is MiscCommands.StopAllDevicesCommand)
				{
					var cmd = (MiscCommands.StopAllDevicesCommand)command;
					Stop();
					result.code = 0;
					break;
				}

				if (command is MiscCommands.GetVersionCommand)
				{
					var cmd = (MiscCommands.GetVersionCommand)command;
					result.SetDatas("v1.2.4", "v2");
					result.code = 0;
					break;
				}

				if (command is MiscCommands.StartUpgradeCommand)
				{
					var cmd = (MiscCommands.StartUpgradeCommand)command;

					break;
				}

				if (command is MiscCommands.TickUpgradeCommand)
				{
					var cmd = (MiscCommands.TickUpgradeCommand)command;

					break;
				}

				if (command is MiscCommands.EndUpgradeCommand)
				{
					var cmd = (MiscCommands.EndUpgradeCommand)command;

					break;
				}
			}
			while (false);
			return result;
		}
	}

	public sealed partial class MainControlPartComponent
	{
		private readonly List<IPartIDComponent> mIDCommandGroup = new List<IPartIDComponent>();

		private List<IPartIDComponent> idComponentGroup
		{
			get
			{
				if (mIDCommandGroup.Count <= 0)
				{
					GetPart().robot.GetIDComponents(mIDCommandGroup);
					mIDCommandGroup.Sort((x, y) => x.deviceID.CompareTo(y.deviceID));
				}
				return mIDCommandGroup;
			}
		}

		private int ChangeID(int targetDevice, int oldID, int newID)
		{
			if (idComponentGroup.Count != 0)
			{
				var idComponentNew = idComponentGroup.Find((item) => { return ((int)item.driversType == targetDevice) && (item.id == oldID); });
				var idComponentOld = idComponentGroup.Find((item) => { return ((int)item.driversType == targetDevice) && (item.id == newID); });
				if (idComponentNew != null)
				{
					idComponentNew.id = newID;
					if (idComponentOld != null)
					{
						idComponentOld.id = oldID;
					}
					return 0;
				}
				else
				{
					return 1;
				}
			}
			return 1;
		}

		private int QueryDevice(ExploreProtocol result)
		{
			var userData = new JsonData();
			userData.SetJsonType(JsonType.Object);

			var devices = new List<int>();
			var deviceDict = new Dictionary<string, List<int>>();

			DriversType driversType = DriversType.None;
			foreach (var v in idComponentGroup)
			{
				driversType = v.driversType;
				string key = v.driversType.ToUUID();

				if (string.IsNullOrEmpty(key))
				{
					continue;
				}

				if (!devices.Contains((int)driversType))
					devices.Add((int)driversType);

				if (!deviceDict.TryGetValue(key, out var idList))
				{
					idList = new List<int>();
					deviceDict[key] = idList;
				}
				idList.Add(v.id);
			}

			userData["drivers"] = LitJsonHelper.CreateArrayJsonData(devices);
			userData["uuid"] = result.uuid;
			foreach (var item in deviceDict)
			{
				userData[item.Key] = LitJsonHelper.CreateArrayJsonData(item.Value);
			}
			result.userData = userData;
			return 0;
		}

		private bool mStopping = false;
		public override void Stop()
		{
			if (mStopping) return;
			mStopping = true;

			GetPart().robot.Stop();

			mStopping = false;
		}
	}
}
