using System;
using Loki;
using UnityEngine;

namespace Ubtrobot
{
	[Serializable]
	public struct EnvRangArgument
	{
		public enum ArgumentType
		{
			Temperature,
			Humidity,
			Luminance,
			SoundVolume,
			WallCount,
			AffectRadius
		}

		public ArgumentType argumentTypetype;
		public float min;
		public float max;
		public bool wholeNumbers;

		private string DescFmtOnUI()
		{
			switch (argumentTypetype)
			{
				case ArgumentType.Temperature: return UILanguageText.temperature;
				case ArgumentType.Humidity: return UILanguageText.humidty;
				case ArgumentType.Luminance: return UILanguageText.luminance;
				case ArgumentType.SoundVolume: return UILanguageText.soundVolume;
				case ArgumentType.WallCount: return UILanguageText.wallCount;
				case ArgumentType.AffectRadius: return UILanguageText.affectRadius;
			}

			return "";
		}

		public string GetDesc(Environment env)
		{
			try
			{
				return string.Format(DescFmtOnUI(), GetEnvironment(env).ToString("f1"));
			}
			catch (Exception e)
			{
				DebugUtility.LogException(e);
			}

			return string.Empty;
		}

		public float GetEnvironment(Environment env, float defValue = 0.0f)
		{
			switch (argumentTypetype)
			{
				case ArgumentType.Temperature:
					{
						return env.temperature;
					}
				case ArgumentType.Luminance:
					{
						return env.luminance;
					}
				case ArgumentType.Humidity:
					{
						return env.humidity;
					}
				case ArgumentType.SoundVolume:
					{
						return env.soundVolume;
					}
				case ArgumentType.WallCount:
					{
						return env.affectCount;
					}
				case ArgumentType.AffectRadius:
					{
						return env.affectRadius * 100.0f;
					}
			}

			return defValue;
		}

		public void UpdateEnvironment(Environment env, float value)
		{
			switch (argumentTypetype)
			{
				case ArgumentType.Temperature:
					{
						env.temperature = value;
						break;
					}
				case ArgumentType.Luminance:
					{
						env.luminance = value;
						break;
					}
				case ArgumentType.Humidity:
					{
						env.humidity = value;
						break;
					}
				case ArgumentType.SoundVolume:
					{
						env.soundVolume = value;
						break;
					}
				case ArgumentType.WallCount:
					{
						env.affectCount = value;
						break;
					}
				case ArgumentType.AffectRadius:
					{
						env.affectRadius = value/100.0f;
						break;
					}
			}

		}
	}

	[Serializable]
	public class EnvRange
	{
		public static readonly EnvRange Default = new EnvRange();

		public bool showEnvParameter = false;

		public EnvRangArgument[] rangArgs;

		[Tooltip("FOV增量")] public float incAmountOfFOV = 20f;

		public float minOfFOV = 30f;

		public float maxOfFOV = 130f;

		/// <summary>
		///  temp code
		/// </summary>
		public string robotNameOnUI;
	}
}
