using System;
using System.Collections.Generic;
using LitJson;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	public class CommandSupportInfo : ILitJson
	{
		private readonly int mDeviceID;
		private readonly List<int> mSupportModes;

		public int deviceID => mDeviceID;
		public List<int> supportModes => mSupportModes;

		public CommandSupportInfo(int deviceID)
		{
			this.mDeviceID = deviceID;
			mSupportModes = new List<int>();
		}

		public JsonData ToJsonData()
		{
			var result = new JsonData();
			result.SetJsonType(JsonType.Object);

			var device = new JsonData();
			device.SetJsonType(JsonType.Object);
			device["deviceID"] = deviceID;
			device["supported"] = LitJsonHelper.CreateArrayJsonData(supportModes);

			result["device"] = device;
			return result;
		}
	}

	#region Explorer 命令
	public partial class CommandFactory
	{
		private static readonly Dictionary<int, Func<ExploreProtocol, ICommand>> msExplorerCreators = new Dictionary<int, Func<ExploreProtocol, ICommand>>()
		{
			{ 1, CreateDevice1Command },
			{ 2, CreateDevice2Command },
			{ 3, CreateDevice3Command },
			{ 4, CreateDevice4Command },
			{ 5, CreateDevice5Command },
			{ 6, CreateDevice6Command },
			{ 7, CreateDevice7Command },
			{ 8, CreateDevice8Command },
			{ 9, CreateDevice9Command },
			{ 10, CreateDevice10Command },
			{ 11, CreateDevice11Command },
		};

		public static ICommand CreateCommand(IProtocol protocol)
		{
			ICommand command = null;

			if (protocol is ExploreProtocol)
			{
				command = CreateCommand((ExploreProtocol)protocol);
			}

			if (command == null)
			{
				DebugUtility.LogError(LoggerTags.Project, "Failed to create command, Unknown protocol : {0}", protocol.ToString());
			}

			return command;
		}

		public static ICommand CreateCommand(ExploreProtocol protocol)
		{
			int device = protocol.device;
			int mode = protocol.mode;
			int id = protocol.id;
			var datas = protocol.datas;

			if (msExplorerCreators.TryGetValue(device, out var creator))
			{
				var result = creator(protocol);
				if (result != null)
				{
					result.host = protocol.host;
					result.device = device;
					result.id = id;
					result.cmdMode = mode;
					result.uuid = protocol.uuid;
					result.debug = protocol.debug;
					return result;
				}
				else
				{
					DebugUtility.LogError(LoggerTags.Project, "Failed to create command, Unsupport protocol: {0}", protocol.ToString());
				}
			}
			else
			{
				DebugUtility.LogError(LoggerTags.Project, "Failed to create command, Missing device: {0}, Unsupport protocol: {1}", device, protocol.ToString());
			}

			return null;
		}

		public static List<CommandSupportInfo> GetCommandSupprtList(Type protocolType, List<CommandSupportInfo> resultCache = null)
		{
			if (resultCache == null)
			{
				resultCache = new List<CommandSupportInfo>();
			}

			if (msDeviceSupportCommands.TryGetValue(protocolType, out var dict))
			{
				foreach (var item in dict)
				{
					var cmdInfo = new CommandSupportInfo(item.Key);
					if (TryGetDeviceSupportList(protocolType, cmdInfo.deviceID, out var modes))
					{
						cmdInfo.supportModes.AddRange(modes);
					}
					resultCache.Add(cmdInfo);
				}
			}

			return resultCache;
		}
	}

	public partial class CommandFactory
	{
		/// 舵机
		/// </summary>
		/// <param name="protocol"></param>
		/// <returns></returns>
		private static ICommand CreateDevice1Command(ExploreProtocol protocol)
		{
			switch (protocol.mode)
			{
				case 127://舵机轮模式
					{
						// forward
						//		0 : forward
						//		1 : backward
						// velocity 0~255
						if (protocol.GetParam(0, out var fowrad) && protocol.GetParam(1, out var velocity))
						{
							return ServoCommands.RotationAlwaysCommand.New(fowrad, velocity);
						}
						break;
					}
				case 128://舵机角模式
					{
						if (protocol.GetParam(0, out var angle) && protocol.GetParam(1, out var duration))
						{
							// ms => s
							return ServoCommands.RotateCommand.New(angle, duration / 1000.0f);
						}
						break;
					}
				case 129://读取舵机角度
					{
						if (protocol.GetParam(0, out var lockAngel))
						{
							return ServoCommands.ReadRotationCommand.New(lockAngel);
						}
						break;
					}
				case 130://停止舵机
					{
						return ServoCommands.StopCommand.New();
					}
			}

			return null;
		}

		/// <summary>
		/// 电机
		/// </summary>
		/// <param name="protocol"></param>
		/// <returns></returns>
		private static ICommand CreateDevice2Command(ExploreProtocol protocol)
		{
			switch (protocol.mode)
			{
				case 127://电机恒速设置
					{
						if (protocol.GetParam(0, out var velocity))
						{
							return MotorCommands.VelocitySettingCommand.New(velocity);
						}
						break;
					}
				case 128://电机PWM设置
					{
						if (protocol.GetParam(0, out var velocity))
						{
							return MotorCommands.PWMSettingCommand.New(velocity);
						}
						break;
					}
				case 129://读取电机速度
					{
						return MotorCommands.ReadVelocityCommand.New();
						break;
					}
				case 130://停止电机
					{
						return MotorCommands.StopCommand.New();
						break;
					}
			}

			return null;
		}

		/// <summary>
		/// 眼灯
		/// </summary>
		/// <param name="protocol"></param>
		/// <returns></returns>
		private static ICommand CreateDevice3Command(ExploreProtocol protocol)
		{
			switch (protocol.mode)
			{
				case 127://亮起眼灯
					{
						if (protocol.GetParamAsRGB(0, out var rgb))
						{
							return EyeCommands.EnableLightCommand.New(rgb);
						}
						break;
					}
				case 128://眼灯表情
					{
						if (protocol.GetParam(0, out var ambilightMode) && protocol.GetParamAsRGB(1, out var rgb) && protocol.GetParam(4, out var count))
						{
							return EyeCommands.ExpressionCommand.New(ambilightMode, rgb, count, false);
						}
						break;
					}
				case 129://眼灯情景灯
					{
						if (protocol.GetParam(0, out var ambilightMode) && protocol.GetParam(1, out var count))
						{
							return EyeCommands.PredefineLightColorCommand.New(ambilightMode, count, false);
						}
						break;
					}
				case 130://关闭眼灯
					{
						return EyeCommands.DisableLightCommand.New();
						break;
					}
				case 131://自定义眼灯
					{
						if (protocol.GetParam(0, out var idx0) &&
							protocol.GetParam(1, out var idx1) &&
							protocol.GetParam(2, out var idx2) &&
							protocol.GetParam(3, out var idx3) &&
							protocol.GetParam(4, out var idx4) &&
							protocol.GetParam(5, out var idx5) &&
							protocol.GetParam(6, out var idx6) &&
							protocol.GetParam(7, out var idx7) &&
							protocol.GetParam(8, out var duration))
						{
							return EyeCommands.CustomLightCommand.New(duration / 1000.0f, idx0, idx1, idx2, idx3, idx4, idx5, idx6, idx7);
						}
						break;
					}
				case 132://眼灯表情阻塞
					{
						if (protocol.GetParam(0, out var ambilightMode) && protocol.GetParamAsRGB(1, out var rgb) && protocol.GetParam(4, out var count))
						{
							return EyeCommands.ExpressionCommand.New(ambilightMode, rgb, count, true);
						}
						break;
					}
				case 133://情景灯阻塞
					{
						if (protocol.GetParam(0, out var ambilightMode) && protocol.GetParam(1, out var count))
						{
							return EyeCommands.PredefineLightColorCommand.New(ambilightMode, count, true);
						}
						break;
					}
			}

			return null;
		}

		/// <summary>
		/// uKit
		/// </summary>
		/// <param name="protocol"></param>
		/// <returns></returns>
		private static ICommand CreateDevice4Command(ExploreProtocol protocol)
		{
			switch (protocol.mode)
			{
				case 127://超声波
					{
						return UKitCommands.UltrasonicCommand.New();
						break;
					}
				case 128://红外
					{
						return UKitCommands.InfraredCommand.New();
						break;
					}
				case 129://按压
					{
						return UKitCommands.ButtonCommand.New();
						break;
					}
				case 130://亮度
					{
						return UKitCommands.LuminanceCommand.New();
						break;
					}
				case 131://声音
					{
						return UKitCommands.SoundCommand.New();
						break;
					}
				case 132://温湿度
					{
						if (protocol.GetParam(0, out int tmode))
						{
							return UKitCommands.TemperatureCommand.New(tmode);
						}
						break;
					}
				case 133://颜色识别模式
					{
						if (protocol.GetParam(0, out int cmode))
						{
							return UKitCommands.ColorRecognitionCommand.New(cmode);
						}
						break;
					}
				case 134://颜色RGB模式
					{
						return UKitCommands.ColorRecognitionRGBCommand.New();
						break;
					}
				case 135://启用超声波灯光
					{
						if (protocol.GetParamAsRGB(0, out var rgb))
						{
							return UKitCommands.EnableUltrasonicLightCommand.New(rgb);
						}
						break;
					}
				case 136://禁用超声波灯光
					{
						return UKitCommands.DisableUltrasonicLightCommand.New();
						break;
					}
			}

			return null;
		}

		/// <summary>
		/// 声音
		/// </summary>
		/// <param name="protocol"></param>
		/// <returns></returns>
		private static ICommand CreateDevice5Command(ExploreProtocol protocol)
		{
			switch (protocol.mode)
			{
				case 127://播放音调
					{
						if (protocol.GetParam(0, out var pitch) && protocol.GetParam(1, out var duration))
						{
							return SoundCommands.PlayPitchCommand.New(pitch, duration / 1000.0f);
						}
						break;
					}
				case 128://播放频率
					{
						if (protocol.GetParam(0, out var frequency) && protocol.GetParam(1, out var duration))
						{
							return SoundCommands.PlayFrequencyCommand.New(frequency, duration / 1000.0f);
						}
						break;
					}
				case 129://结束播放
					{
						return SoundCommands.StopCommand.New();
						break;
					}
			}

			return null;
		}

		/// <summary>
		/// 板载RGB
		/// </summary>
		/// <param name="protocol"></param>
		/// <returns></returns>
		private static ICommand CreateDevice6Command(ExploreProtocol protocol)
		{

			switch (protocol.mode)
			{
				case 127://启用RGB Led
					{
						if (protocol.GetParamAsRGB(0, out var color))
						{
							return RGBLEDCommands.EnableRGBLedCommand.New(color);
						}
						break;
					}
				case 128://禁用RGB Led
					{
						return RGBLEDCommands.DisableRGBLedCommand.New();
						break;
					}
			}

			return null;
		}

		/// <summary>
		/// 电池
		/// </summary>
		/// <param name="protocol"></param>
		/// <returns></returns>
		private static ICommand CreateDevice7Command(ExploreProtocol protocol)
		{
			switch (protocol.mode)
			{
				case 127:// 电池电压
					{
						return BatteryCommands.BatteryVoltageCommand.New();
						break;
					}
			}

			return null;
		}

		private static ICommand CreateDevice8Command(ExploreProtocol protocol)
		{
			switch (protocol.mode)
			{
				case 127://巡线传感器
					{
						if (protocol.GetParam(0, out var gray))
						{
							return PatrolSensorCommands.PatrolSensorBasicCommand.New(gray);
						}
						break;
					}
			}

			return null;
		}

		private static ICommand CreateDevice9Command(ExploreProtocol protocol)
		{
			switch (protocol.mode)
			{
				case 127://巡线传感器
					{
						return GyroCommands.GyroFusionAngleCommand.New();
						break;
					}
			}

			return null;
		}

		private static ICommand CreateDevice10Command(ExploreProtocol protocol)
		{
			switch (protocol.mode)
			{
				case 127://板载按键
					{
						return KeyFeedbackCommands.KeyPressedFeedbackCommand.New();
						break;
					}
			}

			return null;
		}

		/// <summary>
		/// Misc工具以及其他控制命令
		/// </summary>
		/// <param name="protocol"></param>
		/// <returns></returns>
		private static ICommand CreateDevice11Command(ExploreProtocol protocol)
		{
			switch (protocol.mode)
			{
				case 127://修改ID
					{
						if (protocol.GetParam(0, out var targetDevice) && protocol.GetParam(1, out var oldID) && protocol.GetParam(2, out var newID))
						{
							return MiscCommands.SetIDCommand.New(targetDevice, oldID, newID);
						}
						break;
					}
				case 128://检查外设
					{
						//if (protocol.GetParam(0, out var targetDevice))
						//	return MiscCommands.QueryDeviceCommand.New(targetDevice);
						return MiscCommands.QueryDeviceCommand.New(11);
						break;
					}
				case 130://停止所有设备
					{
						return MiscCommands.StopAllDevicesCommand.New();
						break;
					}
			}

			return null;
		}
	}
	#endregion


	#region 支持的命令列表
	public partial class CommandFactory
	{
		/// <summary>
		/// 这里不适合做查表法, 否则代码看起来很影响可读性, 所以分开了支持命令列表以及switch方式创建命令
		/// </summary>
		private static readonly Dictionary<Type, Dictionary<int, int[]>> msDeviceSupportCommands = new Dictionary<Type, Dictionary<int, int[]>>()
		{
			{
				typeof(ExploreProtocol),
				new Dictionary<int, int[]>()
				{
					{ 1, new[] { 127, 128, 129, 130, } },
					{ 2, new[] { 127, 128, 129, 130, 131, 132, 133, } },
					{ 3, new[] { 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, } },
					{ 4, new[] { 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, } },
					{ 5, new[] { 127, 128, 129, } },
					{ 6, new[] { 127, 128, } },
					{ 7, new[] { 127, } },
					{ 8, new[] { 127, } },
					{ 9, new[] { 127, } },
					{ 10, new[] { 127, } },
					{ 11, new[] { 127, 128, 130, } },
				}
			}


		};

		public static bool TryGetDeviceSupportList(Type protocolType, int device, out int[] supported)
		{
			if (msDeviceSupportCommands.TryGetValue(protocolType, out var dict))
			{
				return dict.TryGetValue(device, out supported);
			}
			supported = new int[0];
			return false;
		}

	}
	#endregion
}
