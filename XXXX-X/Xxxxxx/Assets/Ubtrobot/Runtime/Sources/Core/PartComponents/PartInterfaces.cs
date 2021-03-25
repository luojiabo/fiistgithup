using System;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	public enum DeviceType
	{
		None = 0,
		Servo = 1,
		Motor = 2,
		Eyes = 3,
		UKit = 4,
		Sound = 5,
		BRGBLed = 6,
		Battery = 7,
		PatrolSensor = 8,
		Gyro = 9,
		KeyFeedback = 10,
		MainCtrl = 11,
	}

	public enum DriversType
	{
		None = 0,
		Servo,
		Motor,
		UKitInfraredSensor,
		UKitUltrasonicSensor,
		EyesLight,
		UKitButtonSensor,
		UKitLuminanceSensor,
		Sound,
		UKitTemperatureAndHumidity,
		UKitColorRecognitionSensor,
	}

	public static class DriversTypeExtension
	{
		public static string ToUUID(this DriversType driversType)
		{
			switch (driversType)
			{
				case DriversType.Servo: return "servo";
				case DriversType.Motor: return "motor";
				case DriversType.EyesLight: return "eyelamp";
				case DriversType.UKitUltrasonicSensor: return "ultrasonic";
				case DriversType.UKitInfraredSensor: return "ir";
				case DriversType.UKitButtonSensor: return "touch";
				case DriversType.UKitLuminanceSensor: return "light";
				case DriversType.Sound: return "sound";
				case DriversType.UKitTemperatureAndHumidity: return "tah";
				case DriversType.UKitColorRecognitionSensor: return "colors";
				default:
					break;
			}
			return "";
		}
	}

	public static class DeviceTypeExtension
	{
		public static string ToUUID(this DeviceType deviceType)
		{
			switch (deviceType)
			{
				case DeviceType.None:
					break;
				case DeviceType.Servo:
					break;
				case DeviceType.Motor:
					break;
				case DeviceType.Eyes:
					break;
				case DeviceType.UKit:
					break;
				case DeviceType.Sound:
					break;
				case DeviceType.BRGBLed:
					break;
				case DeviceType.Battery:
					break;
				case DeviceType.PatrolSensor:
					break;
				case DeviceType.Gyro:
					break;
				case DeviceType.KeyFeedback:
					break;
				case DeviceType.MainCtrl:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(deviceType), deviceType, null);
			}
			return "";
		}
	}

	public interface IPartComponent : IBoundBox
	{
		IPart owner { get; }
#if UNITY_EDITOR
		void OnInspectorUpdate();
#endif
		void Rewind();
	}

	public interface IPartIDComponent
	{
		int id { get; set; }
		DeviceType deviceID { get; }
		DriversType driversType { get; }
	}

	public interface IPartComponentInput
	{
		void OnRecvInputEvent(InputEventData e);
	}
}
