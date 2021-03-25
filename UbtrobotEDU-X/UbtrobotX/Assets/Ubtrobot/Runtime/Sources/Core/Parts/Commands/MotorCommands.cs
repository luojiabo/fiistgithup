using Loki;

namespace Ubtrobot.MotorCommands
{
	public abstract class MotorCommand : Command
	{
		public override ECommand commandID { get { return ECommand.MotorCommand; } }
	}

	/// <summary>
	/// 恒速设置
	/// </summary>
	public sealed class VelocitySettingCommand : MotorCommand
	{
		public float velocity { get; set; }
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<VelocitySettingCommand>.defaultInstance.Push(this);
		}

		public static VelocitySettingCommand New(int velocity)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<VelocitySettingCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new VelocitySettingCommand();
				cmd.velocity = velocity;
				return cmd;
			});
		}
	}

	/// <summary>
	/// PWM设置
	/// </summary>
	public sealed class PWMSettingCommand : MotorCommand
	{
		public int pwm { get; set; }

		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<PWMSettingCommand>.defaultInstance.Push(this);
		}

		public static PWMSettingCommand New(int pwm)
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<PWMSettingCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new PWMSettingCommand();
				cmd.pwm = pwm;
				return cmd;
			});
		}
	}

	/// <summary>
	/// 读取电机速度
	/// </summary>
	public sealed class ReadVelocityCommand : MotorCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<ReadVelocityCommand>.defaultInstance.Push(this);
		}

		public static ReadVelocityCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<ReadVelocityCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new ReadVelocityCommand();

				return cmd;
			});
		}
	}

	/// <summary>
	/// 停止电机
	/// </summary>
	public sealed class StopCommand : MotorCommand
	{
		protected override void OnRelease()
		{
			base.OnRelease();
			MemoryPool<StopCommand>.defaultInstance.Push(this);
		}

		public static StopCommand New()
		{
			// for Anti-GC, alloc command from memory pool
			return MemoryPool<StopCommand>.defaultInstance.Pop(cmd =>
			{
				if (cmd == null)
					cmd = new StopCommand();

				return cmd;
			});
		}
	}

}
