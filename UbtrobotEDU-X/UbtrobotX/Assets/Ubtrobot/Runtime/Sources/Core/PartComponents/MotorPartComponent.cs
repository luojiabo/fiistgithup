using System;
using System.Collections.Generic;
using UnityEngine;
using Loki;

namespace Ubtrobot
{
	[DynamicSceneDrawer(sceneTitle = "电机", tooltip = "控制旋转")]
	public sealed partial class MotorPartComponent : RotationPartComponent
	{
		private static readonly HashSet<ECommand> msSupportedCommands = new HashSet<ECommand>
		{
			ECommand.MotorCommand,
		};

		public sealed override DeviceType deviceID => DeviceType.Motor;
		public override DriversType driversType => DriversType.Motor;

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
				case ECommand.MotorCommand:
					{
						result = ExecuteMotorCommands(command);
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

		public override void SetID(int id)
		{
			base.SetID(id);
			// string temp = string.Concat("MOTOR_", id.ToString());
			//if (name != temp)
			//{
			//	name = temp;
			//}
		}

		private IProtocol ExecuteMotorCommands(ICommand command)
		{
			ExploreProtocol result = ExploreProtocol.CreateResponse(ProtocolCode.Failure, command);
			do
			{
				if (command is MotorCommands.VelocitySettingCommand)
				{
					var cmd = (MotorCommands.VelocitySettingCommand)command;
					jobStatus = WorkStatus.RotationAlways;
					var job = (RotationAlwaysJob)currentJob;
					job.velocity = cmd.velocity;
					result.code = 0;
					break;
				}

				if (command is MotorCommands.PWMSettingCommand)
				{
					var cmd = (MotorCommands.PWMSettingCommand)command;
					jobStatus = WorkStatus.RotationAlways;
					var job = (RotationAlwaysJob)currentJob;
					job.velocity = cmd.pwm;
					result.code = 0;
					break;
				}

				if (command is MotorCommands.ReadVelocityCommand)
				{
					var cmd = (MotorCommands.ReadVelocityCommand)command;
					int v = 0;
					if (jobStatus == WorkStatus.RotationAlways)
					{
						var job = (RotationAlwaysJob)currentJob;
						v = (int)job.velocity;
					}
					result.SetDatas((int)v);
					result.code = 0;
					break;
				}

				if (command is MotorCommands.StopCommand)
				{
					var cmd = (MotorCommands.StopCommand)command;
					Stop();
					result.code = 0;
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

		public override void Stop()
		{
			jobStatus = WorkStatus.Idle;
		}

#if UNITY_EDITOR
		protected override void DoDrawGizmos()
		{
			var settings = UbtrobotSettings.GetOrLoad().gizmosSettings;
			GizmosUtility.DrawMeshs(gameObject, settings.drawMotor, settings.motorColor);
		}
#endif
	}

	public sealed partial class MotorPartComponent
	{
		public enum WorkStatus
		{
			Idle,
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

			public virtual void Tick(MotorPartComponent servo, float deltaTime) { }
		}

		class IdleJob : PartJob
		{
		}

		class RotationAlwaysJob : PartJob
		{
			public float velocity = 0.0f;

			public override void Tick(MotorPartComponent part, float deltaTime)
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
			new RotationAlwaysJob(),
		};

		private WorkStatus mJobStatus = WorkStatus.Idle;

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
					current.Begin();
				}
			}
		}

		private PartJob currentJob
		{
			get
			{
				return mJobs[(int)mJobStatus];
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
