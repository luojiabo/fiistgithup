using System;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	/// <summary>
	/// The Servo part component 
	/// </summary>
	[DynamicSceneDrawer(sceneTitle = "舵机", tooltip = "控制旋转")]
	public sealed partial class ServoPartComponent : RotationPartComponent
	{
		private static readonly HashSet<ECommand> msSupportedCommands = new HashSet<ECommand>
		{
			ECommand.ServoCommand,
		};

		public sealed override DeviceType deviceID => DeviceType.Servo;
		public override DriversType driversType => DriversType.Servo;

		public override void Rewind()
		{
			jobStatus = WorkStatus.Idle;
			base.Rewind();
		}

		public override ICommandResponseAsync Execute(ICommand command)
		{
			IProtocol result = null;
			switch (command.commandID)
			{
				case ECommand.ServoCommand:
					{
						result = ExecuteServoCommands(command);
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

		private IProtocol ExecuteServoCommands(ICommand command)
		{
			ExploreProtocol result = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command);
			do
			{
				// 舵机角模式
				if (command is ServoCommands.RotateCommand)
				{
					var cmd = (ServoCommands.RotateCommand)command;

					jobStatus = WorkStatus.Rotation;
					var job = (RotationJob)currentJob;
					job.accumulationTime = 0.0f;
					job.duration = cmd.duration;
					var rawAngle = localEulerAnglesY;
					rawAngle = Misc.Convert(convertMode, rawAngle);
					job.startAngle = rawAngle;
					job.endAngle = cmd.angel;
					result.code = 0;
					break;
				}

				// 舵机轮模式
				if (command is ServoCommands.RotationAlwaysCommand)
				{
					var cmd = (ServoCommands.RotationAlwaysCommand)command;
					jobStatus = WorkStatus.RotationAlways;
					var job = (RotationAlwaysJob)currentJob;
					if (cmd.forward)
						job.velocity = cmd.velocity;
					else
						job.velocity = cmd.velocity * -1.0f;
					result.code = 0;
					break;
				}

				// 停止舵机
				if (command is ServoCommands.StopCommand)
				{
					var cmd = (ServoCommands.StopCommand)command;
					Stop();
					result.code = 0;
					break;
				}

				// 读取舵机角度
				if (command is ServoCommands.ReadRotationCommand)
				{
					var cmd = (ServoCommands.ReadRotationCommand)command;
					if (!cmd.enable)
					{
						jobStatus = WorkStatus.Idle;
					}

					Vector2 angleRange = new Vector2(-118.0f, 118.0f);
					float angle = localEulerAnglesY;
					angle = Misc.Convert(convertMode, angle);
					ProtocolCode code = ProtocolCode.Success;
					if (angle < angleRange.x || angle > angleRange.y)
					{
						code = ProtocolCode.Failure;
					}
					result.SetDatas((int)angle);
					result.code = code == ProtocolCode.Success ? 0 : 1;
					break;
				}
			} while (false);

			return result;
		}

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
			var job = currentJob;
			if (job.IsWorking())
			{
				job.Tick(this, deltaTime);
			}
			else
			{
				jobStatus = WorkStatus.Idle;
				job = currentJob;
				job.Tick(this, deltaTime);
			}
		}

		public override void SetID(int id)
		{
			base.SetID(id);
			// name = string.Concat("SERVO_", id.ToString());
		}

		public override void Stop()
		{
			jobStatus = WorkStatus.Idle;
		}

#if UNITY_EDITOR
		protected override void DoDrawGizmos()
		{
			var settings = UbtrobotSettings.GetOrLoad().gizmosSettings;
			if (settings == null)
				return;
			GizmosUtility.DrawMeshs(gameObject, settings.drawServo, settings.servoColor);
		}
#endif
	}

	public sealed partial class ServoPartComponent
	{
		public enum WorkStatus
		{
			Idle,
			Rotation,
			RotationAlways,
		}

		abstract class PartJob
		{
			private bool mValid = false;

			public virtual bool IsWorking()
			{
				return mValid;
			}

			public virtual void Begin()
			{
				mValid = true;
			}

			public virtual void End()
			{
				mValid = false;
			}

			public virtual void Tick(ServoPartComponent servo, float deltaTime) { }
		}

		class IdleJob : PartJob
		{
		}

		class RotationJob : PartJob
		{
			/// <summary>
			/// 开始时的角度
			/// </summary>
			public float startAngle;
			/// <summary>
			/// 目标静止角
			/// </summary>
			public float endAngle;
			/// <summary>
			/// 持续时长
			/// </summary>
			public float duration;
			/// <summary>
			/// 经过时长
			/// </summary>
			public float accumulationTime;

			public override void End()
			{
				base.End();
				endAngle = startAngle = 0.0f;
				duration = accumulationTime = 0.0f;
			}

			public override void Tick(ServoPartComponent part, float deltaTime)
			{
				base.Tick(part, deltaTime);
				if (duration > 0.0f || !Misc.Nearly(endAngle, startAngle, 0.0001f))
				{
					accumulationTime = Mathf.Clamp(accumulationTime + deltaTime, 0.0f, duration);
					part.localEulerAnglesY = startAngle + (endAngle - startAngle) * (accumulationTime / duration);
				}
				else
				{
					part.localEulerAnglesY = endAngle;
				}
				if (accumulationTime >= duration || duration <= 0.0f)
				{
					End();
				}
			}
		}

		class RotationAlwaysJob : PartJob
		{
			public float velocity = 0.0f;

			public override void Tick(ServoPartComponent part, float deltaTime)
			{
				base.Tick(part, deltaTime);
				part.localEulerAnglesY += velocity * deltaTime;
				if (velocity.NearlyZero())
				{
					End();
				}
			}
		}

		private readonly PartJob[] mJobs = new PartJob[]
		{
			new IdleJob(),
			new RotationJob(),
			new RotationAlwaysJob(),
		};

		private WorkStatus mJobStatus = WorkStatus.Idle;

		private PartJob currentJob
		{
			get
			{
				return mJobs[(int)mJobStatus];
			}
		}

		public WorkStatus jobStatus
		{
			get
			{
				return mJobStatus;
			}
			set
			{
				if (mJobStatus != value)
				{
					var current = mJobs[(int)mJobStatus];
					if (current.IsWorking())
						current.End();
					mJobStatus = value;
					current = mJobs[(int)mJobStatus];
					current.Begin();
				}
				else
				{
					var current = mJobs[(int)mJobStatus];
					if (!current.IsWorking())
						current.Begin();
				}
			}
		}

		public override float rotatedSpeed
		{
			get
			{
				if (mJobStatus != WorkStatus.RotationAlways) return base.rotatedSpeed;
				var job = (RotationAlwaysJob)currentJob;
				return job.velocity;
			}
		}
	}
}
