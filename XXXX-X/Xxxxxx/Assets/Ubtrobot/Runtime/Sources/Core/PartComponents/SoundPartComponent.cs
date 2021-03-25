using System;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	[DynamicSceneDrawer(sceneTitle = "扬声器", tooltip = "播放声音")]
	public class SoundPartComponent : AdvancedPartComponent
	{
		private static readonly HashSet<ECommand> msSupportedCommands = new HashSet<ECommand>
		{
			ECommand.SoundCommand,
		};

		public sealed override DeviceType deviceID => DeviceType.Sound;
		public override DriversType driversType => DriversType.Sound;

		[SerializeField]
		protected AudioSource mAudioSource = null;

		public float duration { get; private set; } = 0.0f;

		public override bool Verify(ICommand command)
		{
			if (command.id != id && command.id != 0)
				return false;

			if (!msSupportedCommands.Contains(command.commandID))
				return false;

			return true;
		}

		protected override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (duration <= 0.0f)
				return;

			duration -= deltaTime;
			if (duration <= 0.0f)
			{
				StopPlay();
			}
		}

		public override ICommandResponseAsync Execute(ICommand command)
		{
			IProtocol result = null;
			switch (command.commandID)
			{
				case ECommand.SoundCommand:
					{
						result = ExecuteSoundCommands(command);
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

		/// <summary>
		///	 todo
		///	 通过查表获取clip(音频剪辑)
		/// </summary>
		private void PlayPitch(int pitch, float duration)
		{
			this.duration = duration;
			//if (mAudioSource != null)
			//{
			//	mAudioSource.clip = ;
			//	mAudioSource.Play();
			//}
		}

		/// <summary>
		///	todo
		/// 通过频率生成剪辑， 注意GC问题
		/// </summary>
		private void PlayFrequency(int pitch, float duration)
		{
			this.duration = duration;

			//if (mAudioSource != null)
			//{
			//	mAudioSource.clip = ;
			//	mAudioSource.Play();
			//}
		}

		private void StopPlay() 
		{
			this.duration = 0.0f;
			if (mAudioSource != null)
			{
				mAudioSource.clip = null;
				mAudioSource.Stop();
			}
		}

		private IProtocol ExecuteSoundCommands(ICommand command)
		{
			ExploreProtocol result = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command);
			do
			{
				// 播放音调
				if (command is SoundCommands.PlayPitchCommand)
				{
					var cmd = (SoundCommands.PlayPitchCommand)command;
					PlayPitch(cmd.pitch, cmd.duration);
					result.code = 0;
					break;
				}

				// 播放频率
				if (command is SoundCommands.PlayFrequencyCommand)
				{
					var cmd = (SoundCommands.PlayFrequencyCommand)command;
					PlayFrequency(cmd.frequency, cmd.duration);
					result.code = 0;
					break;
				}

				// 停止播放
				if (command is SoundCommands.StopCommand)
				{
					// var cmd = (SoundCommands.StopCommand)command;
					StopPlay();
					result.code = 0;
					break;
				}

			}
			while (false);
			return result;
		}

		public override void Stop()
		{
			StopPlay();
		}
	}
}
